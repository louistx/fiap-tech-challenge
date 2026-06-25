using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.EntregarOS;

public class EntregarOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<EntregarOSCommand> _validator;

    public EntregarOSService(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<EntregarOSCommand> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
    }

    public bool EntregarOS(EntregarOSCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        os.TransicionarPara(eStatusOS.Entregue);
        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }
}
