namespace TechChallenge.Domain.Entities;

public class Veiculo
{
    public Guid Id { get; set; }
    public string Placa { get; set; }
    public string Modelo { get; set; }
    public string Cor { get; set; }
    public string Marca { get; set; }
    public decimal Valor { get; set; }
}