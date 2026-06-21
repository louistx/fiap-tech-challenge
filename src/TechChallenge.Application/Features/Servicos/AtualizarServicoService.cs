using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Servicos;

public class AtualizarServicoService
{
    private readonly IServicoRepository _servicoRepository;

    public AtualizarServicoService(IServicoRepository servicoRepository)
    {
        _servicoRepository = servicoRepository;
    }

    public bool AtualizarServico(AtualizarServicoCommand command)
    {
        var servico = _servicoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (servico is null)
            throw new KeyNotFoundException($"Serviço com Id {command.Id} não encontrado.");

        servico.Descricao = command.Descricao;
        servico.Valor = command.Valor;

        _servicoRepository.UpdateAsync(servico).GetAwaiter().GetResult();
        return true;
    }
}
