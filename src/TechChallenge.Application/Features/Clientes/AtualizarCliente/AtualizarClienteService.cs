using FluentValidation;
using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

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

        var cliente = _clienteRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente com Id {command.Id} não encontrado.");

        cliente.Nome = command.Nome;
        cliente.Cpf = command.Cpf;
        cliente.Rg = command.Rg;
        cliente.Endereco ??= new Endereco { Id = Guid.NewGuid() };
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
}
