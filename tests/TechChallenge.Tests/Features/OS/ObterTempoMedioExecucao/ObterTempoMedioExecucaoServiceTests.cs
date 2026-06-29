using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.ObterTempoMedioExecucao;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Tests.Features.OS.ObterTempoMedioExecucao;

public class ObterTempoMedioExecucaoServiceTests
{
    [Fact]
    public void DeveCalcularTempoMedioDasOrdensFinalizadas()
    {
        var inicio = new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetFinalizadasComDataFinalizacaoAsync())
            .ReturnsAsync(
            [
                new OrdemServico { DataCriacao = inicio, DataFinalizacao = inicio.AddHours(2) },
                new OrdemServico { DataCriacao = inicio, DataFinalizacao = inicio.AddHours(4) }
            ]);
        var service = new ObterTempoMedioExecucaoService(repository.Object);

        var resultado = service.ObterTempoMedioExecucao();

        resultado.QuantidadeOrdensFinalizadas.Should().Be(2);
        resultado.TempoMedioHoras.Should().Be(3);
        resultado.TempoMedioMinutos.Should().Be(180);
    }

    [Fact]
    public void DeveRetornarZeroQuandoNaoExistiremOrdensFinalizadas()
    {
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetFinalizadasComDataFinalizacaoAsync())
            .ReturnsAsync([]);
        var service = new ObterTempoMedioExecucaoService(repository.Object);

        var resultado = service.ObterTempoMedioExecucao();

        resultado.QuantidadeOrdensFinalizadas.Should().Be(0);
        resultado.TempoMedioHoras.Should().Be(0);
        resultado.TempoMedioMinutos.Should().Be(0);
    }
}
