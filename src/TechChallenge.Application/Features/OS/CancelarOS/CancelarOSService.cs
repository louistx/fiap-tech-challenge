using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.CancelarOS;

public class CancelarOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<CancelarOSCommand> _validator;

    public CancelarOSService(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<CancelarOSCommand> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
    }

    public bool CancelarOS(CancelarOSCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        os.TransicionarPara(StatusOS.Cancelada);
        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }
}
