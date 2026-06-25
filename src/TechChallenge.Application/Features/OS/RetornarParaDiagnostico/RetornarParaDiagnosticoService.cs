using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.RetornarParaDiagnostico;

public class RetornarParaDiagnosticoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<RetornarParaDiagnosticoCommand> _validator;

    public RetornarParaDiagnosticoService(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<RetornarParaDiagnosticoCommand> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
    }

    public bool RetornarParaDiagnostico(RetornarParaDiagnosticoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        os.TransicionarPara(eStatusOS.EmDiagnostico);
        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }
}
