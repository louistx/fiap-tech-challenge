namespace TechChallenge.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
