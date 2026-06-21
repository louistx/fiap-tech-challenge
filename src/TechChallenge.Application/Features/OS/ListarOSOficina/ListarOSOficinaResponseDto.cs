namespace TechChallenge.Application.Features.OS.ListarOSOficina;

public class ListarOSOficinaResponseDto
{
    public string PlacaVeiculo { get; set; }
    public string NomeMecanico { get; set; }
    public string RelatolInicial { get; set; }
}

public class ListarOSOficinaService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;

    public ListarOSOficinaService(IOrdemServicoRepository ordemServicoRepository)
    {
        _ordemServicoRepository = ordemServicoRepository;
    }

    // RF12: apenas OS com status Em Diagnostico, retorna infos basicas
    public IEnumerable<ListarOSOficinaResponseDto> ListarOSOficina(ListarOSOficinaQuery query)
    {
        throw new NotImplementedException();
    }
}