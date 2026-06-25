using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.AprovarOrcamento;

public class AprovarOrcamentoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<AprovarOrcamentoCommand> _validator;

    public AprovarOrcamentoService(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<AprovarOrcamentoCommand> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
    }

    public bool AprovarOrcamento(AprovarOrcamentoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        os.TransicionarPara(eStatusOS.EmExecucao);
        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }
}
