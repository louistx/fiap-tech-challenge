namespace TechChallenge.Application.Features.OS.RegistrarDiagnostico;

public class RegistrarDiagnosticoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IInventarioRepository _inventarioRepository;

    public RegistrarDiagnosticoService(
        IOrdemServicoRepository ordemServicoRepository,
        IInventarioRepository inventarioRepository)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _inventarioRepository = inventarioRepository;
    }

    // RF11 + RF - Verificacao de Estoque
    public bool RegistrarDiagnostico(RegistrarDiagnosticoCommand command)
    {
        
    }
}