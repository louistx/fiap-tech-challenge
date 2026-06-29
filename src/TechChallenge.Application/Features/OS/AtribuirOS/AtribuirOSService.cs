using System;
using FluentValidation;
using TechChallenge.Domain.Enums;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.OS.AtribuirOS;

public class AtribuirOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IValidator<AtribuirOSCommand> _validator;

    public AtribuirOSService(
        IOrdemServicoRepository ordemServicoRepository,
        IFuncionarioRepository funcionarioRepository,
        IValidator<AtribuirOSCommand> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _funcionarioRepository = funcionarioRepository;
        _validator = validator;
    }

    public bool AtribuirOS(AtribuirOSCommand command)
    {
        _validator.ValidateAndThrow(command);

        var mecanico = _funcionarioRepository.GetByIdAsync(command.MecanicoId).GetAwaiter().GetResult();
        if (mecanico is null)
            throw new KeyNotFoundException($"Mecânico com Id {command.MecanicoId} não encontrado.");

        // RF10: mecânico só pode ter 1 OS ativa por vez
        var osAtiva = _ordemServicoRepository.GetOSAtivaMecanicoAsync(command.MecanicoId).GetAwaiter().GetResult();
        if (osAtiva is not null)
            throw new InvalidOperationException($"Mecânico já possui uma OS ativa (Id: {osAtiva.Id}).");

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        os.FuncionarioResponsavelId = command.MecanicoId;
        os.TransicionarPara(StatusOS.EmDiagnostico);

        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }
}
