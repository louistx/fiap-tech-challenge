using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Veiculos.ListarVeiculos;

public class ListarVeiculosService
{
    private readonly IVeiculoRepository _veiculoRepository;

    public ListarVeiculosService(IVeiculoRepository veiculoRepository)
    {
        _veiculoRepository = veiculoRepository;
    }

    public List<Veiculo> ListarVeiculos(ListarVeiculosQuery query)
    {
        return _veiculoRepository.GetAllAsync().GetAwaiter().GetResult();
    }
}
