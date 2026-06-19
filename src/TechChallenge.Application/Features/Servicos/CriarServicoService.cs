using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Servicos;

public class CriarServicoService
{
    private readonly IServicoRepository _servicoRepository;

    public CriarServicoService(IServicoRepository servicoRepository)
    {
        _servicoRepository = servicoRepository;
    }

    public Guid CriarServico(CriarServicoCommand command)
    {
        var servico = new Servico
        {
            Id = Guid.NewGuid(),
            Descricao = command.Descricao,
            Valor = command.Valor
        };

        _servicoRepository.AddAsync(servico).GetAwaiter().GetResult();
        return servico.Id;
    }
}
