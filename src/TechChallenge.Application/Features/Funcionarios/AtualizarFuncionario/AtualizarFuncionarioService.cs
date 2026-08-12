using FluentValidation;
using TechChallenge.Domain.Enums;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Validation;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Application.Features.Funcionarios.AtualizarFuncionario;

public class AtualizarFuncionarioService
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IValidator<AtualizarFuncionarioCommand> _validator;

    public AtualizarFuncionarioService(IFuncionarioRepository funcionarioRepository, IValidator<AtualizarFuncionarioCommand> validator)
    {
        _funcionarioRepository = funcionarioRepository;
        _validator = validator;
    }

    public bool AtualizarFuncionario(AtualizarFuncionarioCommand command)
    {
        _validator.ValidateAndThrow(command);
        var cpf = CpfValidator.Formatar(command.Cpf);

        var funcionario = _funcionarioRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (funcionario is null)
            throw new KeyNotFoundException($"Funcionário com Id {command.Id} não encontrado.");

        if (!Enum.TryParse<TipoFuncionario>(command.Cargo, true, out var tipoFuncionario))
            throw new InvalidOperationException($"Cargo {command.Cargo} inválido.");

        Endereco endereco = new Endereco(funcionario.Endereco.Id, command.Logradouro, command.Complemento, command.Numero, command.Bairro, command.Cidade, command.Estado, command.Cep);

        funcionario = new Funcionario(funcionario.Id, command.Nome, cpf, command.Rg, tipoFuncionario, funcionario.Endereco.Id);

        _funcionarioRepository.UpdateAsync(funcionario).GetAwaiter().GetResult();
        return true;
    }
}
