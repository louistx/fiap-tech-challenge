using System;
using System.Collections.Generic;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Entities;

public class OrdemServico
{
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
}