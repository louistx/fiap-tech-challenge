using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.EntregarOS;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.OS.EntregarOS;

public class EntregarOSServiceTests
{
    [Fact]
    public void DeveEntregarOSQuandoEstiverFinalizada()
    {
        var os = new OrdemServico { Id = Guid.NewGuid(), Status = eStatusOS.Finalizada };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        repository.Setup(repo => repo.UpdateAsync(os)).ReturnsAsync(os);
        var service = new EntregarOSService(repository.Object, new EntregarOSCommandValidator());

        var resultado = service.EntregarOS(new EntregarOSCommand { OrdemServicoId = os.Id });

        resultado.Should().BeTrue();
        os.Status.Should().Be(eStatusOS.Entregue);
        os.DataAtualizacao.Should().NotBeNull();
        repository.Verify(repo => repo.UpdateAsync(os), Times.Once);
    }

    [Fact]
    public void DeveBloquearEntregaQuandoOSNaoEstiverFinalizada()
    {
        var os = new OrdemServico { Id = Guid.NewGuid(), Status = eStatusOS.EmExecucao };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        var service = new EntregarOSService(repository.Object, new EntregarOSCommandValidator());

        var act = () => service.EntregarOS(new EntregarOSCommand { OrdemServicoId = os.Id });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Transição inválida: {eStatusOS.EmExecucao} -> {eStatusOS.Entregue}.");
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }
}
