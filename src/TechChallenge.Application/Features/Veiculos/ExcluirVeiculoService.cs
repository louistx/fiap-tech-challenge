using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Veiculos;

public class ExcluirVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;

    public ExcluirVeiculoService(IVeiculoRepository veiculoRepository)
    {
        _veiculoRepository = veiculoRepository;
    }

    public bool ExcluirVeiculo(Guid id)
    {
        var veiculo = _veiculoRepository.GetByIdAsync(id).GetAwaiter().GetResult();
        if (veiculo is null)
            throw new KeyNotFoundException($"Veículo com Id {id} não encontrado.");

        _veiculoRepository.DeleteAsync(veiculo);
        return true;
    }
}
