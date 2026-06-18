namespace TechChallenge.Application.Features.Clientes;

public class CriarClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public CriarClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public bool CriarCliente(CriarClienteCommand command)
    {
        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = command.Nome,
            Endereco = command.Endereco,
            RG = command.RG,
            Cpf = command.CPF
        };

        _clienteRepository.AddAsync(cliente).GetAwaiter().GetResult();
        return true;
    }
}
