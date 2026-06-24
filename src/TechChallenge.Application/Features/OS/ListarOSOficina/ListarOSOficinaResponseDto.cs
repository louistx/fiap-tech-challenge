using System;
using TechChallenge.Domain.Enums;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.OS.ListarOSOficina;

public class ListarOSOficinaResponseDto
{
    public Guid Id { get; set; }
    public string PlacaVeiculo { get; set; } = string.Empty;
    public string NomeMecanico { get; set; } = string.Empty;
    public string RelatoInicial { get; set; } = string.Empty;
}

public class ListarOSOficinaService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;

    public ListarOSOficinaService(IOrdemServicoRepository ordemServicoRepository)
    {
        _ordemServicoRepository = ordemServicoRepository;
    }

    // RF12: retorna OS Em Diagnóstico com informações básicas para exibição na oficina
    public List<ListarOSOficinaResponseDto> ListarOSOficina()
    {
        var ordens = _ordemServicoRepository.GetByStatusAsync(eStatusOS.EmDiagnostico).GetAwaiter().GetResult();

        return ordens.Select(os => new ListarOSOficinaResponseDto
        {
            Id = os.Id,
            PlacaVeiculo = os.Veiculo.Placa,
            NomeMecanico = os.FuncionarioResponsavel.Nome,
            RelatoInicial = os.Descricao
        }).ToList();
    }
}
