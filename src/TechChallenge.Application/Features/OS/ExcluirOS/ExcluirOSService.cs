using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.OS.ExcluirOS;

public class ExcluirOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<ExcluirOSCommand> _validator;

    public ExcluirOSService(IOrdemServicoRepository ordemServicoRepository, IValidator<ExcluirOSCommand> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
    }

    public bool ExcluirOS(ExcluirOSCommand command)
    {
        _validator.ValidateAndThrow(command);

        var ordemServico = _ordemServicoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (ordemServico is null)
            throw new KeyNotFoundException($"Ordem de serviço com Id {command.Id} não encontrada.");

        _ordemServicoRepository.DeleteAsync(ordemServico).GetAwaiter().GetResult();
        return true;
    }
}
