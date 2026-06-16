namespace TechChallenge.Application.Features.Veiculos;

public class CriarVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;

    public CriarVeiculoService(IVeiculoRepository veiculoRepository)
    {
        _veiculoRepository = veiculoRepository;
    }

    public bool CriarVeiculo(CriarVeiculoCommand command)
    {
        
    }
}