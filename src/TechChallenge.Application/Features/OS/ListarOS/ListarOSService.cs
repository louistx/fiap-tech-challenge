using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.OS.ListarOS;

public class ListarOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;

    public ListarOSService(IOrdemServicoRepository ordemServicoRepository)
    {
        _ordemServicoRepository = ordemServicoRepository;
    }

    public List<OrdemServico> ListarOS(ListarOSQuery query)
    {
        if (query.Status.HasValue)
            return _ordemServicoRepository.GetByStatusAsync(query.Status.Value).GetAwaiter().GetResult();

        return _ordemServicoRepository.GetAllAsync().GetAwaiter().GetResult();
    }
}