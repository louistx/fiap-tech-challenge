using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Entities;

public class OrdemServico
{
    public Guid Id { get; set; }
    public string Descricao { get; set; }
    public eStatusOS Status { get; set; }
    public Guid ClienteResponsavelId { get; set; }
    public Cliente ClienteResponsavel { get; set; }
    public Guid FuncionarioResponsavelId { get; set; }
    public Funcionario FuncionarioResponsavel { get; set; }
    public Guid VeiculoId { get; set; }
    public Veiculo Veiculo { get; set; }
    public ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public DateTime? DataFinalizacao { get; set; }
}