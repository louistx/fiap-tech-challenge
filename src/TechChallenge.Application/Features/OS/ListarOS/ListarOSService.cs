namespace TechChallenge.Application.Features.OS.ListarOS;

public class ListarOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;

    public ListarOSService(IOrdemServicoRepository ordemServicoRepository)
    {
        _ordemServicoRepository = ordemServicoRepository;
    }

    public IEnumerable<object> ListarOS(ListarOSQuery query)
    {
        
    }
}