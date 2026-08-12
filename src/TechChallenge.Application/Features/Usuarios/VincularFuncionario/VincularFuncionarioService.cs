using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Usuarios.VincularFuncionario;

public class VincularFuncionarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IFuncionarioRepository _funcionarioRepository;

    public VincularFuncionarioService(
        IUsuarioRepository usuarioRepository,
        IFuncionarioRepository funcionarioRepository)
    {
        _usuarioRepository = usuarioRepository;
        _funcionarioRepository = funcionarioRepository;
    }

    public void VincularFuncionario(VincularFuncionarioCommand command)
    {
        var usuario = _usuarioRepository.GetByIdAsync(command.UsuarioId).GetAwaiter().GetResult();
        if (usuario is null)
            throw new KeyNotFoundException($"Usuário com Id {command.UsuarioId} não encontrado.");

        var funcionario = _funcionarioRepository.GetByIdAsync(command.FuncionarioId).GetAwaiter().GetResult();
        if (funcionario is null)
            throw new KeyNotFoundException($"Funcionário com Id {command.FuncionarioId} não encontrado.");

        var jaVinculado = _usuarioRepository.ExisteVinculoFuncionarioAsync(command.FuncionarioId).GetAwaiter().GetResult();
        if (jaVinculado && usuario.FuncionarioId != command.FuncionarioId)
            throw new InvalidOperationException($"O funcionário {command.FuncionarioId} já está vinculado a outro usuário.");

        usuario = new Domain.Entities.Usuario(usuario.Id, usuario.Login, usuario.PasswordHash, usuario.TipoUsuario, usuario.Ativo, command.FuncionarioId);

        _usuarioRepository.UpdateAsync(usuario).GetAwaiter().GetResult();
    }
}
