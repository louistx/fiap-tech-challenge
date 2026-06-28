using System;
using FluentValidation;
using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Validation;

namespace TechChallenge.Application.Features.Clientes.CriarCliente;

public class CriarClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<CriarClienteCommand> _validator;

    public CriarClienteService(IClienteRepository clienteRepository, IValidator<CriarClienteCommand> validator)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
    }

    public Guid CriarCliente(CriarClienteCommand command)
    {
        _validator.ValidateAndThrow(command);
        var cpf = CpfValidator.Formatar(command.Cpf);

        var clienteExiste = _clienteRepository.GetByDocumentAsync(cpf).GetAwaiter().GetResult();
        if (clienteExiste is not null)
            throw new InvalidOperationException($"Já existe um cliente cadastrado com o CPF {cpf}.");

        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = command.Nome,
            Rg = command.Rg,
            Cpf = cpf,
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

        _clienteRepository.AddAsync(cliente).GetAwaiter().GetResult();
        return cliente.Id;
    }
}
