namespace TechChallenge.Api.Models.Response;

public class VeiculoResponse
{
    public Guid Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
