using System;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Auth.Sessoes;

public class ListarSessoesService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUser _currentUser;

    public ListarSessoesService(IRefreshTokenRepository refreshTokenRepository, ICurrentUser currentUser)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
    }

    public List<SessaoDto> ListarSessoes()
    {
        if (_currentUser.UsuarioId is not { } usuarioId)
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        var tokens = _refreshTokenRepository
            .GetAtivasDoUsuarioAsync(usuarioId, DateTime.UtcNow)
            .GetAwaiter().GetResult();

        return tokens
            .Select(t => new SessaoDto(t.SessaoId, t.CriadoEm, t.ExpiraEm, t.UserAgent, t.IpCriacao))
            .ToList();
    }
}
