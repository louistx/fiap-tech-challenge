using System;
using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.Veiculos.CriarVeiculo;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.Veiculos.CriarVeiculo;

public class CriarVeiculoServiceTests
{
    [Fact]
    public void DeveCriarVeiculoQuandoPlacaNaoExistirEClienteForEncontrado()
    {
        var command = CriarCommandValido();
        const string placaFormatada = "ABC-1234";
        Veiculo? veiculoCriado = null;
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        veiculoRepository.Setup(repo => repo.GetByPlacaAsync(placaFormatada))
            .ReturnsAsync((Veiculo?)null);
        clienteRepository.Setup(repo => repo.GetByIdAsync(command.ClienteId))
            .ReturnsAsync(new Cliente { Id = command.ClienteId, Nome = "Maria Cliente" });
        veiculoRepository.Setup(repo => repo.AddAsync(It.IsAny<Veiculo>()))
            .Callback<Veiculo>(veiculo => veiculoCriado = veiculo)
            .Returns<Veiculo>(veiculo => Task.FromResult(veiculo));
        var service = new CriarVeiculoService(
            veiculoRepository.Object,
            clienteRepository.Object,
            new CriarVeiculoCommandValidator());

        var veiculoId = service.CriarVeiculo(command);

        veiculoId.Should().NotBeEmpty();
        veiculoCriado.Should().NotBeNull();
        veiculoCriado.Id.Should().Be(veiculoId);
        veiculoCriado.Placa.Should().Be(placaFormatada);
        veiculoCriado.ClienteId.Should().Be(command.ClienteId);
        veiculoRepository.Verify(repo => repo.GetByPlacaAsync(placaFormatada), Times.Once);
        veiculoRepository.Verify(repo => repo.AddAsync(It.IsAny<Veiculo>()), Times.Once);
    }

    [Fact]
    public void DeveCriarVeiculoMercosulComPlacaNormalizada()
    {
        var command = CriarCommandValido();
        command.Placa = "abc1d23";
        Veiculo? veiculoCriado = null;
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        veiculoRepository.Setup(repo => repo.GetByPlacaAsync("ABC1D23"))
            .ReturnsAsync((Veiculo?)null);
        clienteRepository.Setup(repo => repo.GetByIdAsync(command.ClienteId))
            .ReturnsAsync(new Cliente { Id = command.ClienteId, Nome = "Maria Cliente" });
        veiculoRepository.Setup(repo => repo.AddAsync(It.IsAny<Veiculo>()))
            .Callback<Veiculo>(veiculo => veiculoCriado = veiculo)
            .Returns<Veiculo>(veiculo => Task.FromResult(veiculo));
        var service = new CriarVeiculoService(
            veiculoRepository.Object,
            clienteRepository.Object,
            new CriarVeiculoCommandValidator());

        service.CriarVeiculo(command);

        veiculoCriado.Should().NotBeNull();
        veiculoCriado.Placa.Should().Be("ABC1D23");
    }

    [Fact]
    public void DeveImpedirCriacaoQuandoPlacaJaEstiverCadastrada()
    {
        var command = CriarCommandValido();
        command.Placa = "abc1234";
        const string placaFormatada = "ABC-1234";
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        veiculoRepository.Setup(repo => repo.GetByPlacaAsync(placaFormatada))
            .ReturnsAsync(new Veiculo { Id = Guid.NewGuid(), Placa = placaFormatada });
        var service = new CriarVeiculoService(
            veiculoRepository.Object,
            clienteRepository.Object,
            new CriarVeiculoCommandValidator());

        var act = () => service.CriarVeiculo(command);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Já existe um veículo cadastrado com a placa {placaFormatada}.");
        clienteRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        veiculoRepository.Verify(repo => repo.AddAsync(It.IsAny<Veiculo>()), Times.Never);
    }

    [Fact]
    public void DeveRetornarErroQuandoClienteNaoForEncontrado()
    {
        var command = CriarCommandValido();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        veiculoRepository.Setup(repo => repo.GetByPlacaAsync("ABC-1234"))
            .ReturnsAsync((Veiculo?)null);
        clienteRepository.Setup(repo => repo.GetByIdAsync(command.ClienteId))
            .ReturnsAsync((Cliente?)null);
        var service = new CriarVeiculoService(
            veiculoRepository.Object,
            clienteRepository.Object,
            new CriarVeiculoCommandValidator());

        var act = () => service.CriarVeiculo(command);

        act.Should().Throw<KeyNotFoundException>()
            .WithMessage($"Cliente com Id {command.ClienteId} não encontrado.");
        veiculoRepository.Verify(repo => repo.AddAsync(It.IsAny<Veiculo>()), Times.Never);
    }

    private static CriarVeiculoCommand CriarCommandValido()
    {
        return new CriarVeiculoCommand
        {
            Tipo = TipoVeiculo.Carro,
            Placa = "ABC1234",
            Modelo = "Civic",
            Marca = "Honda",
            Cor = "Prata",
            Ano = 2022,
            Quilometragem = 10000,
            Valor = 90000,
            ClienteId = Guid.NewGuid()
        };
    }
}
