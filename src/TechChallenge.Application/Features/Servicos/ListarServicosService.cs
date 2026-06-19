using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Servicos;

public class ListarServicosService
{
    private readonly IServicoRepository _servicoRepository;

    public ListarServicosService(IServicoRepository servicoRepository)
    {
        _servicoRepository = servicoRepository;
    }

    public List<Servico> ListarServicos()
    {
        return _servicoRepository.GetAllAsync().GetAwaiter().GetResult();
    }
}
