using System;
using FluentValidation;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Validation;

namespace TechChallenge.Application.Features.Funcionarios.CriarFuncionario;

public class CriarFuncionarioService
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IValidator<CriarFuncionarioCommand> _validator;

    public CriarFuncionarioService(IFuncionarioRepository funcionarioRepository, IValidator<CriarFuncionarioCommand> validator)
    {
        _funcionarioRepository = funcionarioRepository;
        _validator = validator;
    }

    public Guid CriarFuncionario(CriarFuncionarioCommand command)
    {
        _validator.ValidateAndThrow(command);
        var cpf = CpfValidator.Formatar(command.Cpf);

        var funcionarioExiste = _funcionarioRepository.GetByDocumentAsync(cpf).GetAwaiter().GetResult();
        if (funcionarioExiste is not null)
            throw new InvalidOperationException($"Já existe um funcionário cadastrado com o CPF {cpf}.");

        if (!Enum.TryParse<TipoFuncionario>(command.Cargo, true, out var tipoFuncionario))
            throw new InvalidOperationException($"Cargo {command.Cargo} inválido.");

        var funcionario = new Funcionario
        {
            Id = Guid.NewGuid(),
            Nome = command.Nome,
            Cpf = cpf,
            Rg = command.Rg,
            TipoFuncionario = tipoFuncionario,
            Endereco = new Endereco
            {
                Id = Guid.NewGuid(),
                Logradouro = command.Logradouro,
                Complemento = command.Complemento,
                Numero = command.Numero,
                Bairro = command.Bairro,
                Cidade = command.Cidade,
                Estado = command.Estado,
                Cep = command.Cep
            }
        };

        _funcionarioRepository.AddAsync(funcionario).GetAwaiter().GetResult();
        return funcionario.Id;
    }
}
