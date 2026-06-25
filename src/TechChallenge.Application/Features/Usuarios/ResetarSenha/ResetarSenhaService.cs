using System;
using FluentValidation;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Usuarios.ResetarSenha;

public class ResetarSenhaService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<ResetarSenhaCommand> _validator;

    public ResetarSenhaService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IValidator<ResetarSenhaCommand> validator)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public void ResetarSenha(ResetarSenhaCommand command)
    {
        _validator.ValidateAndThrow(command);

        var usuario = _usuarioRepository.GetByIdAsync(command.UsuarioId).GetAwaiter().GetResult();
        if (usuario is null)
            throw new KeyNotFoundException($"Usuário com Id {command.UsuarioId} não encontrado.");

        usuario.PasswordHash = _passwordHasher.Hash(command.NovaSenha);
        _usuarioRepository.UpdateAsync(usuario).GetAwaiter().GetResult();

        _refreshTokenRepository
            .RevogarTodasDoUsuarioAsync(usuario.Id, DateTime.UtcNow)
            .GetAwaiter().GetResult();
    }
}
