using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Clientes.ObterCliente;

public class ObterClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ObterClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public Cliente ObterCliente(ObterClienteQuery query)
    {
        var cliente = _clienteRepository.GetByIdAsync(query.Id).GetAwaiter().GetResult();
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente com Id {query.Id} não encontrado.");

        return cliente;
    }
}
