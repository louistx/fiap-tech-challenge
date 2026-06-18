namespace TechChallenge.Api.Models.Request;

public class CriarVeiculoRequest
{
    public string Placa { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

public class AtualizarVeiculoRequest
{
    public string Placa { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
