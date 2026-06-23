using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Servicos.ObterServico;

public class ObterServicoService
{
    private readonly IServicoRepository _servicoRepository;

    public ObterServicoService(IServicoRepository servicoRepository)
    {
        _servicoRepository = servicoRepository;
    }

    public Servico ObterServico(ObterServicoQuery query)
    {
        var servico = _servicoRepository.GetByIdAsync(query.Id).GetAwaiter().GetResult();
        if (servico is null)
            throw new KeyNotFoundException($"Serviço com Id {query.Id} não encontrado.");

        return servico;
    }
}
