using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Funcionarios.ExcluirFuncionario;

public class ExcluirFuncionarioService
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<ExcluirFuncionarioCommand> _validator;

    public ExcluirFuncionarioService(
        IFuncionarioRepository funcionarioRepository,
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<ExcluirFuncionarioCommand> validator)
    {
        _funcionarioRepository = funcionarioRepository;
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
    }

    public bool ExcluirFuncionario(ExcluirFuncionarioCommand command)
    {
        _validator.ValidateAndThrow(command);

        var funcionario = _funcionarioRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (funcionario is null)
            throw new KeyNotFoundException($"Funcionário com Id {command.Id} não encontrado.");

        var funcionarioPossuiOrdemServico = _ordemServicoRepository.ExistePorFuncionarioAsync(command.Id).GetAwaiter().GetResult();
        if (funcionarioPossuiOrdemServico)
            throw new InvalidOperationException("Não é possível excluir um funcionário associado a uma ordem de serviço.");

        _funcionarioRepository.DeleteAsync(funcionario).GetAwaiter().GetResult();
        return true;
    }
}
