using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Notifications;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.EnviarOrcamento;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.OS.EnviarOrcamento;

public class EnviarOrcamentoServiceTests
{
    [Fact]
    public void DeveCalcularOrcamentoEEnviarQuandoOSTiverItens()
    {
        var os = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Status = StatusOS.EmDiagnostico,
            Servicos =
            [
                new OrdemServicoServicos { Valor = 100, Quantidade = 2, Acrescimo = 10, Desconto = 5 }
            ],
            Produtos =
            [
                new OrdemServicoProdutos
                {
                    Valor = 50,
                    Quantidade = 3,
                    Produto = new Produto { Descricao = "Filtro", Quantidade = 5 }
                }
            ]
        };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        repository.Setup(repo => repo.UpdateAsync(os)).ReturnsAsync(os);
        var service = new EnviarOrcamentoService(repository.Object, new EnviarOrcamentoCommandValidator());

        var resultado = service.EnviarOrcamento(new EnviarOrcamentoCommand { OrdemServicoId = os.Id });

        resultado.Should().BeTrue();
        os.Valor.Should().Be(355);
        os.Status.Should().Be(StatusOS.AguardandoAprovacao);
        repository.Verify(repo => repo.UpdateAsync(os), Times.Once);
    }

    [Fact]
    public void DeveBloquearEnvioQuandoOSNaoTiverItens()
    {
        var os = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Status = StatusOS.EmDiagnostico
        };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        var service = new EnviarOrcamentoService(repository.Object, new EnviarOrcamentoCommandValidator());

        var act = () => service.EnviarOrcamento(new EnviarOrcamentoCommand { OrdemServicoId = os.Id });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Informe ao menos um serviço ou produto antes de enviar o orçamento.");
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }

    [Fact]
    public void DeveNotificarQuandoEstoqueForInsuficienteAntesDoEnvio()
    {
        var mecanicoId = Guid.NewGuid();
        var os = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Status = StatusOS.EmDiagnostico,
            FuncionarioResponsavelId = mecanicoId,
            Produtos =
            [
                new OrdemServicoProdutos
                {
                    Produto = new Produto { Descricao = "Filtro", Quantidade = 1 },
                    Quantidade = 2
                }
            ]
        };
        var repository = new Mock<IOrdemServicoRepository>();
        var notificationService = new Mock<INotificationService>();
        repository.Setup(repo => repo.GetByIdAsync(os.Id)).ReturnsAsync(os);
        var service = new EnviarOrcamentoService(
            repository.Object,
            new EnviarOrcamentoCommandValidator(),
            notificationService.Object);

        var act = () => service.EnviarOrcamento(new EnviarOrcamentoCommand { OrdemServicoId = os.Id });

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
}
