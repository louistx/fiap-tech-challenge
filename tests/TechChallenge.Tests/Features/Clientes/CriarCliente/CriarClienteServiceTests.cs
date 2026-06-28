using System;
using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.Clientes.CriarCliente;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Tests.Features.Clientes.CriarCliente;

public class CriarClienteServiceTests
{
    [Fact]
    public void DeveCriarClienteQuandoCommandForValidoECpfNaoExistir()
    {
        var command = CriarCommandValido();
        const string cpfFormatado = "529.982.247-25";
        Cliente? clienteCriado = null;
        var repository = new Mock<IClienteRepository>();
        repository.Setup(repo => repo.GetByDocumentAsync(cpfFormatado))
            .ReturnsAsync((Cliente?)null);
        repository.Setup(repo => repo.AddAsync(It.IsAny<Cliente>()))
            .Callback<Cliente>(cliente => clienteCriado = cliente)
            .Returns<Cliente>(cliente => Task.FromResult(cliente));
        var service = new CriarClienteService(repository.Object, new CriarClienteCommandValidator());

        var clienteId = service.CriarCliente(command);

        clienteId.Should().NotBeEmpty();
        clienteCriado.Should().NotBeNull();
        clienteCriado.Id.Should().Be(clienteId);
        clienteCriado.Nome.Should().Be(command.Nome);
        clienteCriado.Cpf.Should().Be(cpfFormatado);
        clienteCriado.Endereco.Cidade.Should().Be(command.Cidade);
        repository.Verify(repo => repo.GetByDocumentAsync(cpfFormatado), Times.Once);
        repository.Verify(repo => repo.AddAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public void DeveImpedirCriacaoQuandoCpfJaEstiverCadastrado()
    {
        var command = CriarCommandValido();
        const string cpfFormatado = "529.982.247-25";
        var repository = new Mock<IClienteRepository>();
        repository.Setup(repo => repo.GetByDocumentAsync(cpfFormatado))
            .ReturnsAsync(new Cliente { Id = Guid.NewGuid(), Cpf = cpfFormatado });
        var service = new CriarClienteService(repository.Object, new CriarClienteCommandValidator());

        var act = () => service.CriarCliente(command);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Já existe um cliente cadastrado com o CPF {cpfFormatado}.");
        repository.Verify(repo => repo.AddAsync(It.IsAny<Cliente>()), Times.Never);
    }

    private static CriarClienteCommand CriarCommandValido()
    {
        return new CriarClienteCommand
        {
            Nome = "Maria Cliente",
            Cpf = "52998224725",
            Rg = "123456789",
            Logradouro = "Rua Teste",
            Complemento = "Apto 10",
            Numero = "100",
            Bairro = "Centro",
            Cidade = "Sao Paulo",
            Estado = "SP",
            Cep = "01001000"
        };
    }
}
