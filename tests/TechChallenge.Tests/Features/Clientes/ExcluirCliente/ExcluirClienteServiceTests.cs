using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.Clientes.ExcluirCliente;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.Clientes.ExcluirCliente;

public class ExcluirClienteServiceTests
{
    [Fact]
    public void DeveExcluirClienteQuandoNaoPossuirOrdemServico()
    {
        var cliente = new Cliente(Guid.NewGuid(), string.Empty, TipoDocumento.Cpf, string.Empty, Guid.NewGuid());
        var clienteRepository = new Mock<IClienteRepository>();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        clienteRepository.Setup(repo => repo.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);
        ordemServicoRepository.Setup(repo => repo.ExistePorClienteAsync(cliente.Id)).ReturnsAsync(false);
        clienteRepository.Setup(repo => repo.DeleteAsync(cliente)).Returns(Task.CompletedTask);
        var service = new ExcluirClienteService(
            clienteRepository.Object,
            ordemServicoRepository.Object,
            new ExcluirClienteCommandValidator());

        var resultado = service.ExcluirCliente(new ExcluirClienteCommand { Id = cliente.Id });

        resultado.Should().BeTrue();
        clienteRepository.Verify(repo => repo.DeleteAsync(cliente), Times.Once);
    }

    [Fact]
    public void DeveImpedirExclusaoQuandoClientePossuirOrdemServico()
    {
        var cliente = new Cliente(Guid.NewGuid(), string.Empty, TipoDocumento.Cpf, string.Empty, Guid.NewGuid());
        var clienteRepository = new Mock<IClienteRepository>();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        clienteRepository.Setup(repo => repo.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);
        ordemServicoRepository.Setup(repo => repo.ExistePorClienteAsync(cliente.Id)).ReturnsAsync(true);
        var service = new ExcluirClienteService(
            clienteRepository.Object,
            ordemServicoRepository.Object,
            new ExcluirClienteCommandValidator());

        var act = () => service.ExcluirCliente(new ExcluirClienteCommand { Id = cliente.Id });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Não é possível excluir um cliente associado a uma ordem de serviço.");
        clienteRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Cliente>()), Times.Never);
    }
}
