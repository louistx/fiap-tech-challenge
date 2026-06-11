namespace TechChallenge.Domain.Entities;

public class OrdemServico
{
    public Guid Id { get; set; }
    public string Descricao { get; set; }
    public Cliente ClienteResponsavel { get; set; }
    public Funcionario FuncionarioResponsavel { get; set; }
    public Veiculo Veiculo { get; set; }
    public IEnumerable<Servico> Servicos { get; set; }
    public IEnumerable<Produto> Produtos { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public DateTime? DataFinalizacao { get; set; }

}
