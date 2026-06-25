using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.ReprovarOrcamento;

public class ReprovarOrcamentoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<ReprovarOrcamentoCommand> _validator;

    public ReprovarOrcamentoService(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<ReprovarOrcamentoCommand> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
    }

    public bool ReprovarOrcamento(ReprovarOrcamentoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        os.TransicionarPara(eStatusOS.Reprovada);
        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }
}
