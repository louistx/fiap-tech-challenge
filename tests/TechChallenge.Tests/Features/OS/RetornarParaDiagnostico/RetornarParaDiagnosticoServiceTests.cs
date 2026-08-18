using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.RetornarParaDiagnostico;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.OS.RetornarParaDiagnostico;

public class RetornarParaDiagnosticoServiceTests
{
    [Fact]
    public void DeveRetornarOSReprovadaParaDiagnostico()
    {
        var os = new OrdemServico(Guid.NewGuid(), "Descrição da OS", string.Empty, StatusOS.Reprovada, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, null, null, 0, 0, 0);
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        repository.Setup(repo => repo.UpdateAsync(os)).ReturnsAsync(os);
        var service = new RetornarParaDiagnosticoService(repository.Object, new RetornarParaDiagnosticoCommandValidator());

        var resultado = service.RetornarParaDiagnostico(new RetornarParaDiagnosticoCommand { OrdemServicoId = os.Id });

        resultado.Should().BeTrue();
        os.Status.Should().Be(StatusOS.EmDiagnostico);
        os.DataAtualizacao.Should().NotBeNull();
        repository.Verify(repo => repo.UpdateAsync(os), Times.Once);
    }

    [Fact]
    public void DeveBloquearRetornoQuandoOSNaoEstiverReprovada()
    {
        var os = new OrdemServico(Guid.NewGuid(), "Descrição da OS", string.Empty, StatusOS.AguardandoAprovacao, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, null, null, 0, 0, 0);
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        var service = new RetornarParaDiagnosticoService(repository.Object, new RetornarParaDiagnosticoCommandValidator());

        var act = () => service.RetornarParaDiagnostico(new RetornarParaDiagnosticoCommand { OrdemServicoId = os.Id });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Transição inválida: {StatusOS.AguardandoAprovacao} -> {StatusOS.EmDiagnostico}.");
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }
}
