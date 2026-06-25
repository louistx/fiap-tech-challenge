using System;
using System.Collections.Generic;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Entities;

public class OrdemServico
{
    private static readonly Dictionary<eStatusOS, eStatusOS[]> Transicoes = new()
    {
        [eStatusOS.Recebida] = [eStatusOS.EmDiagnostico, eStatusOS.Cancelada],
        [eStatusOS.EmDiagnostico] = [eStatusOS.AguardandoAprovacao, eStatusOS.Cancelada],
        [eStatusOS.AguardandoAprovacao] = [eStatusOS.EmExecucao, eStatusOS.Reprovada, eStatusOS.Cancelada],
        [eStatusOS.Reprovada] = [eStatusOS.EmDiagnostico, eStatusOS.Cancelada],
        [eStatusOS.EmExecucao] = [eStatusOS.Finalizada, eStatusOS.Cancelada],
        [eStatusOS.Finalizada] = [eStatusOS.Entregue],
        [eStatusOS.Entregue] = [],
        [eStatusOS.Cancelada] = []
    };

    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public eStatusOS Status { get; set; }
    public Guid ClienteResponsavelId { get; set; }
    public Cliente ClienteResponsavel { get; set; } = null!;
    public Guid FuncionarioResponsavelId { get; set; }
    public Funcionario FuncionarioResponsavel { get; set; } = null!;
    public Guid VeiculoId { get; set; }
    public Veiculo Veiculo { get; set; } = null!;
    public ICollection<OrdemServicoServicos> Servicos { get; set; } = new List<OrdemServicoServicos>();
    public ICollection<OrdemServicoProdutos> Produtos { get; set; } = new List<OrdemServicoProdutos>();
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public DateTime? DataFinalizacao { get; set; }
    public double Valor { get; set; }
    public double Desconto { get; set; }
    public double Acrescimo { get; set; }

    public void TransicionarPara(eStatusOS novoStatus)
    {
        if (!Transicoes.TryGetValue(Status, out var permitidos) || !permitidos.Contains(novoStatus))
            throw new InvalidOperationException($"Transição inválida: {Status} -> {novoStatus}.");

        Status = novoStatus;
        DataAtualizacao = DateTime.UtcNow;

        if (novoStatus == eStatusOS.Finalizada)
            DataFinalizacao = DateTime.UtcNow;
    }
}
