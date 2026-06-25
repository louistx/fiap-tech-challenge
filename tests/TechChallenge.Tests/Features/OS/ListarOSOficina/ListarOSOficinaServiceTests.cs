using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.ListarOSOficina;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.OS.ListarOSOficina;

public class ListarOSOficinaServiceTests
{
    [Fact]
    public void DeveListarOSDaOficinaMapeandoCamposBasicos()
    {
        var os = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Status = eStatusOS.EmDiagnostico,
            Descricao = "Motor falhando",
            Veiculo = new Veiculo { Placa = "ABC1D23" },
            FuncionarioResponsavel = new Funcionario { Nome = "Joao Mecanico" }
        };
        var repository = new Mock<IOrdemServicoRepository>();
        repository.Setup(repo => repo.GetByStatusAsync(eStatusOS.EmDiagnostico)).ReturnsAsync([os]);
        var service = new ListarOSOficinaService(repository.Object);

        var resultado = service.ListarOSOficina();

        resultado.Should().ContainSingle();
        resultado[0].Id.Should().Be(os.Id);
        resultado[0].PlacaVeiculo.Should().Be(os.Veiculo.Placa);
        resultado[0].NomeMecanico.Should().Be(os.FuncionarioResponsavel.Nome);
        resultado[0].RelatoInicial.Should().Be(os.Descricao);
        repository.Verify(repo => repo.GetByStatusAsync(eStatusOS.EmDiagnostico), Times.Once);
    }
}
