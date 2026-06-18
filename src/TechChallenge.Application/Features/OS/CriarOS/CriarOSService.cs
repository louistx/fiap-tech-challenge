namespace TechChallenge.Application.Features.OS.CriarOS;

public class CriarOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;

    public CriarOSService(IOrdemServicoRepository ordemServicoRepository)
    {
        _ordemServicoRepository = ordemServicoRepository;
    }

    public bool CriarOS(CriarOSCommand command)
    {

    }
}