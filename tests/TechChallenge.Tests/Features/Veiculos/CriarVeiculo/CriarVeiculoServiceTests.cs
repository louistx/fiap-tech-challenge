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
        Veiculo? veiculoCriado = null;
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        veiculoRepository.Setup(repo => repo.GetByPlacaAsync(command.Placa))
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
        veiculoCriado!.Id.Should().Be(veiculoId);
        veiculoCriado.Placa.Should().Be(command.Placa);
        veiculoCriado.ClienteId.Should().Be(command.ClienteId);
        veiculoRepository.Verify(repo => repo.AddAsync(It.IsAny<Veiculo>()), Times.Once);
    }

    [Fact]
    public void DeveImpedirCriacaoQuandoPlacaJaEstiverCadastrada()
    {
        var command = CriarCommandValido();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        veiculoRepository.Setup(repo => repo.GetByPlacaAsync(command.Placa))
            .ReturnsAsync(new Veiculo { Id = Guid.NewGuid(), Placa = command.Placa });
        var service = new CriarVeiculoService(
            veiculoRepository.Object,
            clienteRepository.Object,
            new CriarVeiculoCommandValidator());

        var act = () => service.CriarVeiculo(command);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Já existe um veículo cadastrado com a placa {command.Placa}.");
        clienteRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        veiculoRepository.Verify(repo => repo.AddAsync(It.IsAny<Veiculo>()), Times.Never);
    }

    [Fact]
    public void DeveRetornarErroQuandoClienteNaoForEncontrado()
    {
        var command = CriarCommandValido();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        veiculoRepository.Setup(repo => repo.GetByPlacaAsync(command.Placa))
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
