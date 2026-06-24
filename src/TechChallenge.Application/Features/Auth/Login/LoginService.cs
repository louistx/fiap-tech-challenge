using System;
using FluentValidation;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Application.Features.Auth.Login;

public class LoginService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IAuthSettings _settings;
    private readonly IValidator<LoginCommand> _validator;

    public LoginService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IAuthSettings settings,
        IValidator<LoginCommand> validator)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _settings = settings;
        _validator = validator;
    }

    public AuthResult Login(LoginCommand command)
    {
        _validator.ValidateAndThrow(command);

        var usuario = _usuarioRepository.GetByLoginAsync(command.Login).GetAwaiter().GetResult();
        if (usuario is null || !usuario.Ativo)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        if (!_passwordHasher.Verify(command.Senha, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var sessaoId = Guid.NewGuid();
        var access = _tokenService.GerarAccessToken(usuario, sessaoId);
        var refreshCru = _tokenService.GerarRefreshToken();

        var agora = DateTime.UtcNow;
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            TokenHash = _tokenService.HashRefreshToken(refreshCru),
            SessaoId = sessaoId,
            CriadoEm = agora,
            ExpiraEm = agora.AddDays(_settings.RefreshTokenDays),
            SessaoExpiraEm = agora.AddDays(_settings.RefreshSessionMaxDays),
            UserAgent = command.UserAgent,
            IpCriacao = command.Ip
        };

        _refreshTokenRepository.AddAsync(refreshToken).GetAwaiter().GetResult();

        return new AuthResult(access.Token, access.ExpiraEm, refreshCru);
    }
}
