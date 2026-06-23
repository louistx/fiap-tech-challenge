using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Servicos.ListarServicos;

public class ListarServicosService
{
    private readonly IServicoRepository _servicoRepository;

    public ListarServicosService(IServicoRepository servicoRepository)
    {
        _servicoRepository = servicoRepository;
    }

    public List<Servico> ListarServicos(ListarServicosQuery query)
    {
        return _servicoRepository.GetAllAsync().GetAwaiter().GetResult();
    }
}
