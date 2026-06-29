using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.FinalizarOS;

public class FinalizarOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<FinalizarOSCommand> _validator;

    public FinalizarOSService(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<FinalizarOSCommand> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
    }

    public bool FinalizarOS(FinalizarOSCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        os.TransicionarPara(StatusOS.Finalizada);
        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }
}
