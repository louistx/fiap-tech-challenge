using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Servicos;

public class ExcluirServicoService
{
    private readonly IServicoRepository _servicoRepository;

    public ExcluirServicoService(IServicoRepository servicoRepository)
    {
        _servicoRepository = servicoRepository;
    }

    public bool ExcluirServico(Guid id)
    {
        var servico = _servicoRepository.GetByIdAsync(id).GetAwaiter().GetResult();
        if (servico is null)
            throw new KeyNotFoundException($"Serviço com Id {id} não encontrado.");

        _servicoRepository.DeleteAsync(servico);
        return true;
    }
}
