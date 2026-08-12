using System;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Usuarios.AlterarStatus;

public class AlterarStatusUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AlterarStatusUsuarioService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public void AlterarStatus(AlterarStatusUsuarioCommand command)
    {
        var usuario = _usuarioRepository.GetByIdAsync(command.UsuarioId).GetAwaiter().GetResult();
        if (usuario is null)
            throw new KeyNotFoundException($"Usuário com Id {command.UsuarioId} não encontrado.");

        usuario = new Domain.Entities.Usuario(usuario.Id, usuario.Login, usuario.PasswordHash, usuario.TipoUsuario, command.Ativo, usuario.FuncionarioId);
        _usuarioRepository.UpdateAsync(usuario).GetAwaiter().GetResult();

        // Desativar derruba todos os refresh tokens ativos.
        if (!command.Ativo)
        {
            _refreshTokenRepository
                .RevogarTodasDoUsuarioAsync(usuario.Id, DateTime.UtcNow)
                .GetAwaiter().GetResult();
        }
    }
}
