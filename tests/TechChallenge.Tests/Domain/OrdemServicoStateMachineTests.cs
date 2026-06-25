using FluentAssertions;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Domain;

public class OrdemServicoStateMachineTests
{
    public static IEnumerable<object[]> TransicoesValidas()
    {
        yield return [eStatusOS.Recebida, eStatusOS.EmDiagnostico];
        yield return [eStatusOS.Recebida, eStatusOS.Cancelada];
        yield return [eStatusOS.EmDiagnostico, eStatusOS.AguardandoAprovacao];
        yield return [eStatusOS.EmDiagnostico, eStatusOS.Cancelada];
        yield return [eStatusOS.AguardandoAprovacao, eStatusOS.EmExecucao];
        yield return [eStatusOS.AguardandoAprovacao, eStatusOS.Reprovada];
        yield return [eStatusOS.AguardandoAprovacao, eStatusOS.Cancelada];
        yield return [eStatusOS.Reprovada, eStatusOS.EmDiagnostico];
        yield return [eStatusOS.Reprovada, eStatusOS.Cancelada];
        yield return [eStatusOS.EmExecucao, eStatusOS.Finalizada];
        yield return [eStatusOS.EmExecucao, eStatusOS.Cancelada];
        yield return [eStatusOS.Finalizada, eStatusOS.Entregue];
    }

    public static IEnumerable<object[]> TransicoesInvalidas()
    {
        yield return [eStatusOS.Recebida, eStatusOS.Finalizada];
        yield return [eStatusOS.EmDiagnostico, eStatusOS.EmExecucao];
        yield return [eStatusOS.AguardandoAprovacao, eStatusOS.Finalizada];
        yield return [eStatusOS.EmExecucao, eStatusOS.Entregue];
        yield return [eStatusOS.Finalizada, eStatusOS.Cancelada];
        yield return [eStatusOS.Entregue, eStatusOS.Cancelada];
        yield return [eStatusOS.Cancelada, eStatusOS.Recebida];
    }

    [Theory]
    [MemberData(nameof(TransicoesValidas))]
    public void DeveTransicionarQuandoMovimentoForValido(eStatusOS origem, eStatusOS destino)
    {
        var os = new OrdemServico { Status = origem };

        os.TransicionarPara(destino);

        os.Status.Should().Be(destino);
        os.DataAtualizacao.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(TransicoesInvalidas))]
    public void DeveBloquearTransicaoInvalida(eStatusOS origem, eStatusOS destino)
    {
        var os = new OrdemServico { Status = origem };

        var act = () => os.TransicionarPara(destino);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Transição inválida: {origem} -> {destino}.");
    }

    [Fact]
    public void DeveDefinirDataFinalizacaoQuandoFinalizar()
    {
        var os = new OrdemServico { Status = eStatusOS.EmExecucao };

        os.TransicionarPara(eStatusOS.Finalizada);

        os.Status.Should().Be(eStatusOS.Finalizada);
        os.DataFinalizacao.Should().NotBeNull();
    }
}
