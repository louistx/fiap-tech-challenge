using System;
using FluentValidation;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
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
        var documento = FormatarDocumento(command.TipoDocumento, command.Documento);

        var clienteExiste = _clienteRepository.GetByDocumentAsync(documento).GetAwaiter().GetResult();
        if (clienteExiste is not null)
            throw new InvalidOperationException($"Já existe um cliente cadastrado com o {NomeDocumento(command.TipoDocumento)} {documento}.");

        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = command.Nome,
            TipoDocumento = command.TipoDocumento,
            Documento = documento,
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

    private static string FormatarDocumento(TipoDocumento tipoDocumento, string documento) => tipoDocumento switch
    {
        TipoDocumento.Cpf => CpfValidator.Formatar(documento),
        TipoDocumento.Cnpj => CnpjValidator.Formatar(documento),
        _ => documento.Trim()
    };

    private static string NomeDocumento(TipoDocumento tipoDocumento) => tipoDocumento switch
    {
        TipoDocumento.Cpf => "CPF",
        TipoDocumento.Cnpj => "CNPJ",
        _ => "RG"
    };
}
