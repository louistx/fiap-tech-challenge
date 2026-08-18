using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.AprovarOrcamento;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.OS.AprovarOrcamento;

public class AprovarOrcamentoServiceTests
{
    [Fact]
    public void DeveAprovarOrcamentoEIniciarExecucao()
    {
        var os = new OrdemServico(Guid.NewGuid(), "Descrição da OS", string.Empty, StatusOS.AguardandoAprovacao, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, null, null, 0, 0, 0);
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        repository.Setup(repo => repo.UpdateAsync(os)).ReturnsAsync(os);
        var service = new AprovarOrcamentoService(repository.Object, new AprovarOrcamentoCommandValidator());

        var resultado = service.AprovarOrcamento(new AprovarOrcamentoCommand { OrdemServicoId = os.Id });

        resultado.Should().BeTrue();
        os.Status.Should().Be(StatusOS.EmExecucao);
        os.DataAtualizacao.Should().NotBeNull();
        repository.Verify(repo => repo.UpdateAsync(os), Times.Once);
    }

    [Fact]
    public void DeveBloquearAprovacaoQuandoOSNaoAguardarAprovacao()
    {
        var os = new OrdemServico(Guid.NewGuid(), "Descrição da OS", string.Empty, StatusOS.EmDiagnostico, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, null, null, 0, 0, 0); var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        var service = new AprovarOrcamentoService(repository.Object, new AprovarOrcamentoCommandValidator());

        var act = () => service.AprovarOrcamento(new AprovarOrcamentoCommand { OrdemServicoId = os.Id });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Transição inválida: {StatusOS.EmDiagnostico} -> {StatusOS.EmExecucao}.");
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }
}
