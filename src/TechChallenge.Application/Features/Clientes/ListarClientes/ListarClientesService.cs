using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Clientes.ListarClientes;

public class ListarClientesService
{
    private readonly IClienteRepository _clienteRepository;

    public ListarClientesService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public List<Cliente> ListarClientes(ListarClientesQuery query)
    {
        return _clienteRepository.GetAllAsync().GetAwaiter().GetResult();
    }
}
