using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Veiculos;

public class ObterVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;

    public ObterVeiculoService(IVeiculoRepository veiculoRepository)
    {
        _veiculoRepository = veiculoRepository;
    }

    public Veiculo ObterVeiculo(Guid id)
    {
        var veiculo = _veiculoRepository.GetByIdAsync(id).GetAwaiter().GetResult();
        if (veiculo is null)
            throw new KeyNotFoundException($"Veículo com Id {id} não encontrado.");

        return veiculo;
    }
}
