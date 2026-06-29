using FluentValidation;
using TechChallenge.Application.Abstractions.Notifications;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Notifications;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.RetornarParaDiagnostico;

public class RetornarParaDiagnosticoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<RetornarParaDiagnosticoCommand> _validator;
    private readonly INotificationService _notificationService;

    public RetornarParaDiagnosticoService(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<RetornarParaDiagnosticoCommand> validator,
        INotificationService? notificationService = null)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
        _notificationService = notificationService ?? NullNotificationService.Instance;
    }

    public bool RetornarParaDiagnostico(RetornarParaDiagnosticoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        var statusAnterior = os.Status;

        os.TransicionarPara(StatusOS.EmDiagnostico);
        _notificationService.NotificarTransicaoOS(os, statusAnterior);
        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }
}
