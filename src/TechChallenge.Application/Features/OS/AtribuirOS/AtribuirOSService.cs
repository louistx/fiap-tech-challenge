using TechChallenge.Domain.Enums;
using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.OS.AtribuirOS;

public class AtribuirOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;

    public AtribuirOSService(IOrdemServicoRepository ordemServicoRepository)
    {
        _ordemServicoRepository = ordemServicoRepository;
    }

    public bool AtribuirOS(AtribuirOSCommand command)
    {
        // RF10: mecânico só pode ter 1 OS ativa por vez
        var osAtiva = _ordemServicoRepository.GetOSAtivaMecanicoAsync(command.MecanicoId).GetAwaiter().GetResult();
        if (osAtiva is not null)
            throw new InvalidOperationException($"Mecânico já possui uma OS em diagnóstico (Id: {osAtiva.Id}).");

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        if (os.Status != eStatusOS.Recebida)
            throw new InvalidOperationException($"Apenas OS com status Recebida podem ser atribuídas. Status atual: {os.Status}.");

        os.FuncionarioResponsavelId = command.MecanicoId;
        os.Status = eStatusOS.EmDiagnostico;
        os.DataAtualizacao = DateTime.UtcNow;

        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }
}