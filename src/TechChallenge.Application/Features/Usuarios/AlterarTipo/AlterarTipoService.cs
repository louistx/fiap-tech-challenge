using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Usuarios.AlterarTipo;

public class AlterarTipoService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AlterarTipoService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public void AlterarTipo(AlterarTipoCommand command)
    {
        var usuario = _usuarioRepository.GetByIdAsync(command.UsuarioId).GetAwaiter().GetResult();
        if (usuario is null)
            throw new KeyNotFoundException($"Usuário com Id {command.UsuarioId} não encontrado.");

        usuario = new Domain.Entities.Usuario(usuario.Id, usuario.Login, usuario.PasswordHash, command.TipoUsuario, usuario.Ativo, usuario.FuncionarioId);
        _usuarioRepository.UpdateAsync(usuario).GetAwaiter().GetResult();
    }
}
