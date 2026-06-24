using FluentValidation;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Application.Features.Usuarios.CriarUsuario;

public class CriarUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<CriarUsuarioCommand> _validator;

    public CriarUsuarioService(
        IUsuarioRepository usuarioRepository,
        IFuncionarioRepository funcionarioRepository,
        IPasswordHasher passwordHasher,
        IValidator<CriarUsuarioCommand> validator)
    {
        _usuarioRepository = usuarioRepository;
        _funcionarioRepository = funcionarioRepository;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public Guid CriarUsuario(CriarUsuarioCommand command)
    {
        _validator.ValidateAndThrow(command);

        var loginExiste = _usuarioRepository.ExisteLoginAsync(command.Login).GetAwaiter().GetResult();
        if (loginExiste)
            throw new InvalidOperationException($"Já existe um usuário com o login '{command.Login}'.");

        if (command.FuncionarioId is { } funcionarioId)
            ValidarVinculo(funcionarioId);

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Login = command.Login,
            PasswordHash = _passwordHasher.Hash(command.Senha),
            TipoUsuario = command.TipoUsuario,
            Ativo = true,
            FuncionarioId = command.FuncionarioId
        };

        _usuarioRepository.AddAsync(usuario).GetAwaiter().GetResult();
        return usuario.Id;
    }

    private void ValidarVinculo(Guid funcionarioId)
    {
        var funcionario = _funcionarioRepository.GetByIdAsync(funcionarioId).GetAwaiter().GetResult();
        if (funcionario is null)
            throw new KeyNotFoundException($"Funcionário com Id {funcionarioId} não encontrado.");

        var jaVinculado = _usuarioRepository.ExisteVinculoFuncionarioAsync(funcionarioId).GetAwaiter().GetResult();
        if (jaVinculado)
            throw new InvalidOperationException($"O funcionário {funcionarioId} já está vinculado a outro usuário.");
    }
}
