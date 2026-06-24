using System;
using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.OS.AtribuirOS;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.OS.AtribuirOS;

public class AtribuirOSServiceTests
{
    [Fact]
    public void DeveAtribuirOSRecebidaParaMecanicoSemOSAtiva()
    {
        var mecanicoId = Guid.NewGuid();
        var ordemServico = new OrdemServico { Id = Guid.NewGuid(), Status = eStatusOS.Recebida };
        var funcionarioRepository = new Mock<IFuncionarioRepository>();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        funcionarioRepository.Setup(repo => repo.GetByIdAsync(mecanicoId))
            .ReturnsAsync(new Funcionario { Id = mecanicoId, TipoFuncionario = eTipoFuncionario.Mecanico });
        ordemServicoRepository.Setup(repo => repo.GetOSAtivaMecanicoAsync(mecanicoId))
            .ReturnsAsync((OrdemServico?)null);
        ordemServicoRepository.Setup(repo => repo.GetByIdAsync(ordemServico.Id))
            .ReturnsAsync(ordemServico);
        ordemServicoRepository.Setup(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()))
            .Returns<OrdemServico>(os => Task.FromResult(os));
        var service = new AtribuirOSService(
            ordemServicoRepository.Object,
            funcionarioRepository.Object,
            new AtribuirOSCommandValidator());

        var resultado = service.AtribuirOS(new AtribuirOSCommand
        {
            OrdemServicoId = ordemServico.Id,
            MecanicoId = mecanicoId
        });

        resultado.Should().BeTrue();
        ordemServico.FuncionarioResponsavelId.Should().Be(mecanicoId);
        ordemServico.Status.Should().Be(eStatusOS.EmDiagnostico);
        ordemServico.DataAtualizacao.Should().NotBeNull();
        ordemServicoRepository.Verify(repo => repo.UpdateAsync(ordemServico), Times.Once);
    }

    [Fact]
    public void DeveImpedirAtribuicaoQuandoMecanicoJaPossuirOSAtiva()
    {
        var mecanicoId = Guid.NewGuid();
        var osAtiva = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Status = eStatusOS.EmDiagnostico,
            FuncionarioResponsavelId = mecanicoId
        };
        var funcionarioRepository = new Mock<IFuncionarioRepository>();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        funcionarioRepository.Setup(repo => repo.GetByIdAsync(mecanicoId))
            .ReturnsAsync(new Funcionario { Id = mecanicoId, TipoFuncionario = eTipoFuncionario.Mecanico });
        ordemServicoRepository.Setup(repo => repo.GetOSAtivaMecanicoAsync(mecanicoId))
            .ReturnsAsync(osAtiva);
        var service = new AtribuirOSService(
            ordemServicoRepository.Object,
            funcionarioRepository.Object,
            new AtribuirOSCommandValidator());

        var act = () => service.AtribuirOS(new AtribuirOSCommand
        {
            OrdemServicoId = Guid.NewGuid(),
            MecanicoId = mecanicoId
        });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Mecânico já possui uma OS em diagnóstico (Id: {osAtiva.Id}).");
        ordemServicoRepository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }

    [Fact]
    public void DeveImpedirAtribuicaoQuandoOSNaoEstiverRecebida()
    {
        var mecanicoId = Guid.NewGuid();
        var ordemServico = new OrdemServico { Id = Guid.NewGuid(), Status = eStatusOS.Criada };
        var funcionarioRepository = new Mock<IFuncionarioRepository>();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        funcionarioRepository.Setup(repo => repo.GetByIdAsync(mecanicoId))
            .ReturnsAsync(new Funcionario { Id = mecanicoId, TipoFuncionario = eTipoFuncionario.Mecanico });
        ordemServicoRepository.Setup(repo => repo.GetOSAtivaMecanicoAsync(mecanicoId))
            .ReturnsAsync((OrdemServico?)null);
        ordemServicoRepository.Setup(repo => repo.GetByIdAsync(ordemServico.Id))
            .ReturnsAsync(ordemServico);
        var service = new AtribuirOSService(
            ordemServicoRepository.Object,
            funcionarioRepository.Object,
            new AtribuirOSCommandValidator());

        var act = () => service.AtribuirOS(new AtribuirOSCommand
        {
            OrdemServicoId = ordemServico.Id,
            MecanicoId = mecanicoId
        });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Apenas OS com status Recebida podem ser atribuídas. Status atual: {ordemServico.Status}.");
        ordemServicoRepository.Verify(repo => repo.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }
}
