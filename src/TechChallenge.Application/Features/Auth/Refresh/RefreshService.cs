using System;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Application.Features.Auth.Refresh;

public class RefreshService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IAuthSettings _settings;

    public RefreshService(
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        IAuthSettings settings)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _settings = settings;
    }

    public AuthResult Refresh(RefreshCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            throw new UnauthorizedAccessException("Refresh token inválido.");

        var agora = DateTime.UtcNow;
        var hash = _tokenService.HashRefreshToken(command.RefreshToken);
        var token = _refreshTokenRepository.GetByHashAsync(hash).GetAwaiter().GetResult();

        if (token is null)
            throw new UnauthorizedAccessException("Refresh token inválido.");

        var jaRotacionado = token.RevogadoEm is not null || token.SubstituidoPorId is not null;
        if (jaRotacionado)
        {
            var dentroDoOverlap =
                token.MotivoRevogacao == "rotacionado"
                && token.RevogadoEm is { } revogadoEm
                && (agora - revogadoEm).TotalSeconds <= _settings.RefreshOverlapSeconds;

            if (!dentroDoOverlap)
            {
                // Reuso detectado: derruba a sessão inteira (RFC 9700).
                _refreshTokenRepository
                    .RevogarSessaoAsync(token.SessaoId, "reuso-detectado", agora)
                    .GetAwaiter().GetResult();
                throw new UnauthorizedAccessException("Refresh token inválido.");
            }
            // dentro do overlap: tolera concorrência e rotaciona normalmente
        }
        else if (agora >= token.ExpiraEm || agora >= token.SessaoExpiraEm)
        {
            throw new UnauthorizedAccessException("Refresh token expirado.");
        }

        var usuario = token.Usuario;
        if (usuario is null || !usuario.Ativo)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var access = _tokenService.GerarAccessToken(usuario, token.SessaoId);
        var novoCru = _tokenService.GerarRefreshToken();

        var sessaoExpira = token.SessaoExpiraEm;
        var expira = agora.AddDays(_settings.RefreshTokenDays);
        if (expira > sessaoExpira) expira = sessaoExpira;

        var novo = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            TokenHash = _tokenService.HashRefreshToken(novoCru),
            SessaoId = token.SessaoId,
            CriadoEm = agora,
            ExpiraEm = expira,
            SessaoExpiraEm = sessaoExpira,
            UserAgent = token.UserAgent,
            IpCriacao = token.IpCriacao
        };

        if (token.RevogadoEm is null)
        {
            token.RevogadoEm = agora;
            token.MotivoRevogacao = "rotacionado";
            token.SubstituidoPorId = novo.Id;
        }

        _refreshTokenRepository.AddAsync(novo).GetAwaiter().GetResult();
        _refreshTokenRepository.UpdateAsync(token).GetAwaiter().GetResult();

        return new AuthResult(access.Token, access.ExpiraEm, novoCru);
    }
}
