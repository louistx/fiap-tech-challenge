using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Veiculos;

public class ListarVeiculosService
{
    private readonly IVeiculoRepository _veiculoRepository;

    public ListarVeiculosService(IVeiculoRepository veiculoRepository)
    {
        _veiculoRepository = veiculoRepository;
    }

    public List<Veiculo> ListarVeiculos()
    {
        return _veiculoRepository.GetAllAsync().GetAwaiter().GetResult();
    }
}
