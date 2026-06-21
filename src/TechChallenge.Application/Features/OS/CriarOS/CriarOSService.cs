using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.OS.CriarOS;

public class CriarOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;

    public CriarOSService(IOrdemServicoRepository ordemServicoRepository)
    {
        _ordemServicoRepository = ordemServicoRepository;
    }

    public Guid CriarOS(CriarOSCommand command)
    {
        var os = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Descricao = command.Descricao,
            // RF04: ao ser criada fica Criada, depois transiciona para Recebida (fila)
            Status = eStatusOS.Recebida,
            ClienteResponsavelId = command.ClienteResponsavelId,
            FuncionarioResponsavelId = command.FuncionarioResponsavelId,
            VeiculoId = command.VeiculoId,
            DataCriacao = DateTime.UtcNow
        };

        _ordemServicoRepository.AddAsync(os).GetAwaiter().GetResult();
        return os.Id;
    }
}