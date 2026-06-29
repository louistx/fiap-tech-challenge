using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Notifications;
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
        var produto = new Produto { Id = Guid.NewGuid(), Descricao = "Filtro", Quantidade = 4 };
        var os = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Status = StatusOS.EmExecucao,
            Produtos = [new OrdemServicoProdutos { Produto = produto, Quantidade = 2 }]
        };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        repository.Setup(repo => repo.UpdateAsync(os)).ReturnsAsync(os);
        var service = new FinalizarOSService(repository.Object, new FinalizarOSCommandValidator());

        var resultado = service.FinalizarOS(new FinalizarOSCommand { OrdemServicoId = os.Id });

        resultado.Should().BeTrue();
        os.Status.Should().Be(StatusOS.Finalizada);
        produto.Quantidade.Should().Be(2);
        os.DataFinalizacao.Should().NotBeNull();
        repository.Verify(repo => repo.UpdateAsync(os), Times.Once);
    }

    [Fact]
    public void DeveBloquearFinalizacaoQuandoEstoqueForInsuficiente()
    {
        var mecanicoId = Guid.NewGuid();
        var os = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Status = StatusOS.EmExecucao,
            FuncionarioResponsavelId = mecanicoId,
            Produtos =
            [
                new OrdemServicoProdutos
                {
                    Produto = new Produto { Id = Guid.NewGuid(), Descricao = "Filtro", Quantidade = 1 },
                    Quantidade = 2
                }
            ]
        };
        var repository = new Mock<IOrdemServicoRepository>();
        var notificationService = new Mock<INotificationService>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        var service = new FinalizarOSService(
            repository.Object,
            new FinalizarOSCommandValidator(),
            notificationService.Object);

        var act = () => service.FinalizarOS(new FinalizarOSCommand { OrdemServicoId = os.Id });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Estoque insuficiente para o produto Filtro.");
        notificationService.Verify(service => service.NotificarFuncionariosPorFuncao(
            TipoFuncionario.Administrador,
            "Estoque insuficiente",
            It.Is<string>(mensagem => mensagem.Contains(os.Id.ToString()) && mensagem.Contains("Filtro"))), Times.Once);
        notificationService.Verify(service => service.NotificarFuncionario(
            mecanicoId,
            "Estoque insuficiente",
            It.Is<string>(mensagem => mensagem.Contains(os.Id.ToString()) && mensagem.Contains("Filtro"))), Times.Once);
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }

    [Fact]
    public void DeveBloquearFinalizacaoQuandoOSNaoEstiverEmExecucao()
    {
        var os = new OrdemServico { Id = Guid.NewGuid(), Status = StatusOS.AguardandoAprovacao };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        var service = new FinalizarOSService(repository.Object, new FinalizarOSCommandValidator());

        var act = () => service.FinalizarOS(new FinalizarOSCommand { OrdemServicoId = os.Id });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Transição inválida: {StatusOS.AguardandoAprovacao} -> {StatusOS.Finalizada}.");
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }
}
