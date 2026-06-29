using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.ReprovarOrcamento;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.OS.ReprovarOrcamento;

public class ReprovarOrcamentoServiceTests
{
    [Fact]
    public void DeveReprovarOrcamentoQuandoOSAguardarAprovacao()
    {
        var os = new OrdemServico { Id = Guid.NewGuid(), Status = StatusOS.AguardandoAprovacao };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        repository.Setup(repo => repo.UpdateAsync(os)).ReturnsAsync(os);
        var service = new ReprovarOrcamentoService(repository.Object, new ReprovarOrcamentoCommandValidator());

        var resultado = service.ReprovarOrcamento(new ReprovarOrcamentoCommand { OrdemServicoId = os.Id });

        resultado.Should().BeTrue();
        os.Status.Should().Be(StatusOS.Reprovada);
        os.DataAtualizacao.Should().NotBeNull();
        repository.Verify(repo => repo.UpdateAsync(os), Times.Once);
    }

    [Fact]
    public void DeveBloquearReprovacaoForaDeAguardandoAprovacao()
    {
        var os = new OrdemServico { Id = Guid.NewGuid(), Status = StatusOS.EmDiagnostico };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        var service = new ReprovarOrcamentoService(repository.Object, new ReprovarOrcamentoCommandValidator());

        var act = () => service.ReprovarOrcamento(new ReprovarOrcamentoCommand { OrdemServicoId = os.Id });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Transição inválida: {StatusOS.EmDiagnostico} -> {StatusOS.Reprovada}.");
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }
}
