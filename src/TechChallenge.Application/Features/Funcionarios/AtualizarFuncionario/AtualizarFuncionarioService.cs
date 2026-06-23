using FluentValidation;
using TechChallenge.Domain.Enums;
using TechChallenge.Application.Abstractions.Repositories;

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

        var funcionario = _funcionarioRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (funcionario is null)
            throw new KeyNotFoundException($"Funcionário com Id {command.Id} não encontrado.");

        if (!Enum.TryParse<eTipoFuncionario>(command.Cargo, true, out var tipoFuncionario))
            throw new InvalidOperationException($"Cargo {command.Cargo} inválido.");

        funcionario.Nome = command.Nome;
        funcionario.Cpf = command.Cpf;
        funcionario.Rg = command.Rg;
        funcionario.TipoFuncionario = tipoFuncionario;
        funcionario.Endereco.Logradouro = command.Logradouro;
        funcionario.Endereco.Complemento = command.Complemento;
        funcionario.Endereco.Numero = command.Numero;
        funcionario.Endereco.Bairro = command.Bairro;
        funcionario.Endereco.Cidade = command.Cidade;
        funcionario.Endereco.Estado = command.Estado;
        funcionario.Endereco.Cep = command.Cep;

        _funcionarioRepository.UpdateAsync(funcionario).GetAwaiter().GetResult();
        return true;
    }
}
