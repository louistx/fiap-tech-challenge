using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Servicos;

public class ObterServicoService
{
    private readonly IServicoRepository _servicoRepository;

    public ObterServicoService(IServicoRepository servicoRepository)
    {
        _servicoRepository = servicoRepository;
    }

    public Servico ObterServico(Guid id)
    {
        var servico = _servicoRepository.GetByIdAsync(id).GetAwaiter().GetResult();
        if (servico is null)
            throw new KeyNotFoundException($"Serviço com Id {id} não encontrado.");

        return servico;
    }
}
