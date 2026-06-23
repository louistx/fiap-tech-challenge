using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Funcionarios.ExcluirFuncionario;

public class ExcluirFuncionarioService
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IValidator<ExcluirFuncionarioCommand> _validator;

    public ExcluirFuncionarioService(IFuncionarioRepository funcionarioRepository, IValidator<ExcluirFuncionarioCommand> validator)
    {
        _funcionarioRepository = funcionarioRepository;
        _validator = validator;
    }

    public bool ExcluirFuncionario(ExcluirFuncionarioCommand command)
    {
        _validator.ValidateAndThrow(command);

        var funcionario = _funcionarioRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (funcionario is null)
            throw new KeyNotFoundException($"Funcionário com Id {command.Id} não encontrado.");

        _funcionarioRepository.DeleteAsync(funcionario).GetAwaiter().GetResult();
        return true;
    }
}
