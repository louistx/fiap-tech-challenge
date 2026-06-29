using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.ListarOS;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.OS.ListarOS;

public class ListarOSServiceTests
{
    [Fact]
    public void DeveListarTodasAsOSQuandoStatusNaoForInformado()
    {
        var ordens = new List<OrdemServico>
        {
            new() { Id = Guid.NewGuid(), Status = StatusOS.Recebida },
            new() { Id = Guid.NewGuid(), Status = StatusOS.Finalizada }
        };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(ordens);
        var service = new ListarOSService(repository.Object);

        var resultado = service.ListarOS(new ListarOSQuery());

        resultado.Should().BeEquivalentTo(ordens);
        repository.Verify(repo => repo.GetAllAsync(), Times.Once);
        repository.Verify(repo => repo.GetByStatusAsync(It.IsAny<StatusOS>()), Times.Never);
    }

    [Fact]
    public void DeveListarOSPorStatusQuandoStatusForInformado()
    {
        var ordens = new List<OrdemServico>
        {
            new() { Id = Guid.NewGuid(), Status = StatusOS.EmDiagnostico }
        };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByStatusAsync(StatusOS.EmDiagnostico)).ReturnsAsync(ordens);
        var service = new ListarOSService(repository.Object);

        var resultado = service.ListarOS(new ListarOSQuery { Status = StatusOS.EmDiagnostico });

        resultado.Should().BeEquivalentTo(ordens);
        repository.Verify(repo => repo.GetByStatusAsync(StatusOS.EmDiagnostico), Times.Once);
        repository.Verify(repo => repo.GetAllAsync(), Times.Never);
    }
}
