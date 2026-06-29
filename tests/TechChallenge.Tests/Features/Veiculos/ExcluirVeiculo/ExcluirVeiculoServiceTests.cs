using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.Veiculos.ExcluirVeiculo;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Tests.Features.Veiculos.ExcluirVeiculo;

public class ExcluirVeiculoServiceTests
{
    [Fact]
    public void DeveExcluirVeiculoQuandoNaoPossuirOrdemServico()
    {
        var veiculo = new Veiculo { Id = Guid.NewGuid() };
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        veiculoRepository.Setup(repo => repo.GetByIdAsync(veiculo.Id)).ReturnsAsync(veiculo);
        ordemServicoRepository.Setup(repo => repo.ExistePorVeiculoAsync(veiculo.Id)).ReturnsAsync(false);
        veiculoRepository.Setup(repo => repo.DeleteAsync(veiculo)).Returns(Task.CompletedTask);
        var service = new ExcluirVeiculoService(
            veiculoRepository.Object,
            ordemServicoRepository.Object,
            new ExcluirVeiculoCommandValidator());

        var resultado = service.ExcluirVeiculo(new ExcluirVeiculoCommand { Id = veiculo.Id });

        resultado.Should().BeTrue();
        veiculoRepository.Verify(repo => repo.DeleteAsync(veiculo), Times.Once);
    }

    [Fact]
    public void DeveImpedirExclusaoQuandoVeiculoPossuirOrdemServico()
    {
        var veiculo = new Veiculo { Id = Guid.NewGuid() };
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        veiculoRepository.Setup(repo => repo.GetByIdAsync(veiculo.Id)).ReturnsAsync(veiculo);
        ordemServicoRepository.Setup(repo => repo.ExistePorVeiculoAsync(veiculo.Id)).ReturnsAsync(true);
        var service = new ExcluirVeiculoService(
            veiculoRepository.Object,
            ordemServicoRepository.Object,
            new ExcluirVeiculoCommandValidator());

        var act = () => service.ExcluirVeiculo(new ExcluirVeiculoCommand { Id = veiculo.Id });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Não é possível excluir um veículo associado a uma ordem de serviço.");
        veiculoRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Veiculo>()), Times.Never);
    }
}
