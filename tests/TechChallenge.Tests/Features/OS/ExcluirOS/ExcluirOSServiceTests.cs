using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.ExcluirOS;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.OS.ExcluirOS;

public class ExcluirOSServiceTests
{
    [Fact]
    public void DeveExcluirOSQuandoEncontrada()
    {
        var os = new OrdemServico(Guid.NewGuid(), "Descrição da OS", string.Empty, StatusOS.Recebida, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, null, null, 0, 0, 0);
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        repository.Setup(repo => repo.DeleteAsync(os)).Returns(Task.CompletedTask);
        var service = new ExcluirOSService(repository.Object, new ExcluirOSCommandValidator());

        var resultado = service.ExcluirOS(new ExcluirOSCommand { Id = os.Id });

        resultado.Should().BeTrue();
        repository.Verify(repo => repo.DeleteAsync(os), Times.Once);
    }

    [Fact]
    public void DeveLancarQuandoOSNaoForEncontrada()
    {
        var osId = Guid.NewGuid();
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(osId)).ReturnsAsync((OrdemServico?)null);
        var service = new ExcluirOSService(repository.Object, new ExcluirOSCommandValidator());

        var act = () => service.ExcluirOS(new ExcluirOSCommand { Id = osId });

        act.Should().Throw<KeyNotFoundException>()
            .WithMessage($"Ordem de serviço com Id {osId} não encontrada.");
        repository.Verify(repo => repo.DeleteAsync(It.IsAny<OrdemServico>()), Times.Never);
    }
}
