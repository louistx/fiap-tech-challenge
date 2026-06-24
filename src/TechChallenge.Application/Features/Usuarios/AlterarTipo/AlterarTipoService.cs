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

        usuario.TipoUsuario = command.TipoUsuario;
        _usuarioRepository.UpdateAsync(usuario).GetAwaiter().GetResult();
    }
}
