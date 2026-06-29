using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.CancelarOS;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.OS.CancelarOS;

public class CancelarOSServiceTests
{
    [Fact]
    public void DeveCancelarOSQuandoTransicaoForValida()
    {
        var os = new OrdemServico { Id = Guid.NewGuid(), Status = StatusOS.EmDiagnostico };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        repository.Setup(repo => repo.UpdateAsync(os)).ReturnsAsync(os);
        var service = new CancelarOSService(repository.Object, new CancelarOSCommandValidator());

        var resultado = service.CancelarOS(new CancelarOSCommand { OrdemServicoId = os.Id });

        resultado.Should().BeTrue();
        os.Status.Should().Be(StatusOS.Cancelada);
        os.DataAtualizacao.Should().NotBeNull();
        repository.Verify(repo => repo.UpdateAsync(os), Times.Once);
    }

    [Fact]
    public void DeveLancarQuandoOSNaoForEncontrada()
    {
        var osId = Guid.NewGuid();
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(osId)).ReturnsAsync((OrdemServico?)null);
        var service = new CancelarOSService(repository.Object, new CancelarOSCommandValidator());

        var act = () => service.CancelarOS(new CancelarOSCommand { OrdemServicoId = osId });

        act.Should().Throw<KeyNotFoundException>()
            .WithMessage($"OS com Id {osId} não encontrada.");
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }

    [Fact]
    public void DeveBloquearCancelamentoDeOSEntregue()
    {
        var os = new OrdemServico { Id = Guid.NewGuid(), Status = StatusOS.Entregue };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        var service = new CancelarOSService(repository.Object, new CancelarOSCommandValidator());

        var act = () => service.CancelarOS(new CancelarOSCommand { OrdemServicoId = os.Id });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Transição inválida: {StatusOS.Entregue} -> {StatusOS.Cancelada}.");
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }
}
