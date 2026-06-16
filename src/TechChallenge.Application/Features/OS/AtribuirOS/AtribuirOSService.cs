namespace TechChallenge.Application.Features.OS.AtribuirOS;

public class AtribuirOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;

    public AtribuirOSService(IOrdemServicoRepository ordemServicoRepository)
    {
        _ordemServicoRepository = ordemServicoRepository;
    }

    // RF10: mecanico so pode ter 1 OS por vez; status muda para Em Diagnostico
    public bool AtribuirOS(AtribuirOSCommand command)
    {
        
    }
}