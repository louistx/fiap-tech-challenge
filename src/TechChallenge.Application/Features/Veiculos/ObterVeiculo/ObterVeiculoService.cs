using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Veiculos.ObterVeiculo;

public class ObterVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;

    public ObterVeiculoService(IVeiculoRepository veiculoRepository)
    {
        _veiculoRepository = veiculoRepository;
    }

    public Veiculo ObterVeiculo(ObterVeiculoQuery query)
    {
        var veiculo = _veiculoRepository.GetByIdAsync(query.Id).GetAwaiter().GetResult();
        if (veiculo is null)
            throw new KeyNotFoundException($"Veículo com Id {query.Id} não encontrado.");

        return veiculo;
    }
}
