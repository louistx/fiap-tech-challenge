using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Servicos.ExcluirServico;

public class ExcluirServicoService
{
    private readonly IServicoRepository _servicoRepository;
    private readonly IOrdemServicoServicosRepository _ordemServicoServicosRepository;
    private readonly IValidator<ExcluirServicoCommand> _validator;

    public ExcluirServicoService(
        IServicoRepository servicoRepository,
        IOrdemServicoServicosRepository ordemServicoServicosRepository,
        IValidator<ExcluirServicoCommand> validator)
    {
        _servicoRepository = servicoRepository;
        _ordemServicoServicosRepository = ordemServicoServicosRepository;
        _validator = validator;
    }

    public bool ExcluirServico(ExcluirServicoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var servico = _servicoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (servico is null)
            throw new KeyNotFoundException($"Serviço com Id {command.Id} não encontrado.");

        var servicoEmUso = _ordemServicoServicosRepository.ExisteServicoEmOrdemServicoAsync(command.Id).GetAwaiter().GetResult();
        if (servicoEmUso)
            throw new InvalidOperationException("Não é possível excluir um serviço associado a uma ordem de serviço.");

        _servicoRepository.DeleteAsync(servico).GetAwaiter().GetResult();
        return true;
    }
}
