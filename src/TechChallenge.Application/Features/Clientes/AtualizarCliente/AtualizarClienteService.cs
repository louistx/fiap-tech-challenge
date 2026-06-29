using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Validation;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.Clientes.AtualizarCliente;

public class AtualizarClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<AtualizarClienteCommand> _validator;

    public AtualizarClienteService(IClienteRepository clienteRepository, IValidator<AtualizarClienteCommand> validator)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
    }

    public bool AtualizarCliente(AtualizarClienteCommand command)
    {
        _validator.ValidateAndThrow(command);
        var documento = FormatarDocumento(command.TipoDocumento, command.Documento);

        var cliente = _clienteRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente com Id {command.Id} não encontrado.");

        cliente.Nome = command.Nome;
        cliente.TipoDocumento = command.TipoDocumento;
        cliente.Documento = documento;
        cliente.Endereco.Logradouro = command.Logradouro;
        cliente.Endereco.Complemento = command.Complemento;
        cliente.Endereco.Numero = command.Numero;
        cliente.Endereco.Bairro = command.Bairro;
        cliente.Endereco.Cidade = command.Cidade;
        cliente.Endereco.Estado = command.Estado;
        cliente.Endereco.Cep = command.Cep;

        _clienteRepository.UpdateAsync(cliente).GetAwaiter().GetResult();
        return true;
    }

    private static string FormatarDocumento(TipoDocumento tipoDocumento, string documento) => tipoDocumento switch
    {
        TipoDocumento.Cpf => CpfValidator.Formatar(documento),
        TipoDocumento.Cnpj => CnpjValidator.Formatar(documento),
        _ => documento.Trim()
    };
}
