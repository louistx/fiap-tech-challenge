using FluentAssertions;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Domain;

public class OrdemServicoStateMachineTests
{
    public static TheoryData<StatusOS, StatusOS> TransicoesValidas()
    {
        return new TheoryData<StatusOS, StatusOS>
        {
            { StatusOS.Recebida, StatusOS.EmDiagnostico },
            { StatusOS.Recebida, StatusOS.Cancelada },
            { StatusOS.EmDiagnostico, StatusOS.AguardandoAprovacao },
            { StatusOS.EmDiagnostico, StatusOS.Cancelada },
            { StatusOS.AguardandoAprovacao, StatusOS.EmExecucao },
            { StatusOS.AguardandoAprovacao, StatusOS.Reprovada },
            { StatusOS.AguardandoAprovacao, StatusOS.Cancelada },
            { StatusOS.Reprovada, StatusOS.EmDiagnostico },
            { StatusOS.Reprovada, StatusOS.Cancelada },
            { StatusOS.EmExecucao, StatusOS.Finalizada },
            { StatusOS.EmExecucao, StatusOS.Cancelada },
            { StatusOS.Finalizada, StatusOS.Entregue }
        };
    }

    public static TheoryData<StatusOS, StatusOS> TransicoesInvalidas()
    {
        return new TheoryData<StatusOS, StatusOS>
        {
            { StatusOS.Recebida, StatusOS.Finalizada },
            { StatusOS.EmDiagnostico, StatusOS.EmExecucao },
            { StatusOS.AguardandoAprovacao, StatusOS.Finalizada },
            { StatusOS.EmExecucao, StatusOS.Entregue },
            { StatusOS.Finalizada, StatusOS.Cancelada },
            { StatusOS.Entregue, StatusOS.Cancelada },
            { StatusOS.Cancelada, StatusOS.Recebida }
        };
    }

    [Theory]
    [MemberData(nameof(TransicoesValidas))]
    public void DeveTransicionarQuandoMovimentoForValido(StatusOS origem, StatusOS destino)
    {
        var os = new OrdemServico { Status = origem };

        os.TransicionarPara(destino);

        os.Status.Should().Be(destino);
        os.DataAtualizacao.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(TransicoesInvalidas))]
    public void DeveBloquearTransicaoInvalida(StatusOS origem, StatusOS destino)
    {
        var os = new OrdemServico { Status = origem };

        var act = () => os.TransicionarPara(destino);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Transição inválida: {origem} -> {destino}.");
    }

    [Fact]
    public void DeveDefinirDataFinalizacaoQuandoFinalizar()
    {
        var os = new OrdemServico { Status = StatusOS.EmExecucao };

        os.TransicionarPara(StatusOS.Finalizada);

        os.Status.Should().Be(StatusOS.Finalizada);
        os.DataFinalizacao.Should().NotBeNull();
    }
}
