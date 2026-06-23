namespace TechChallenge.Domain.Entities;

public class Servico
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
