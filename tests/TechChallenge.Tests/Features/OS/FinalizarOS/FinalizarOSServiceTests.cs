using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.FinalizarOS;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.OS.FinalizarOS;

public class FinalizarOSServiceTests
{
    [Fact]
    public void DeveFinalizarOSQuandoEstiverEmExecucao()
    {
        var os = new OrdemServico { Id = Guid.NewGuid(), Status = eStatusOS.EmExecucao };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        repository.Setup(repo => repo.UpdateAsync(os)).ReturnsAsync(os);
        var service = new FinalizarOSService(repository.Object, new FinalizarOSCommandValidator());

        var resultado = service.FinalizarOS(new FinalizarOSCommand { OrdemServicoId = os.Id });

        resultado.Should().BeTrue();
        os.Status.Should().Be(eStatusOS.Finalizada);
        os.DataFinalizacao.Should().NotBeNull();
        repository.Verify(repo => repo.UpdateAsync(os), Times.Once);
    }

    [Fact]
    public void DeveBloquearFinalizacaoQuandoOSNaoEstiverEmExecucao()
    {
        var os = new OrdemServico { Id = Guid.NewGuid(), Status = eStatusOS.AguardandoAprovacao };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        var service = new FinalizarOSService(repository.Object, new FinalizarOSCommandValidator());

        var act = () => service.FinalizarOS(new FinalizarOSCommand { OrdemServicoId = os.Id });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Transição inválida: {eStatusOS.AguardandoAprovacao} -> {eStatusOS.Finalizada}.");
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }
}
