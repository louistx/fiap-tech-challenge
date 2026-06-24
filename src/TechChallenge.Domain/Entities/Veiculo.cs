using System;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Entities;

public class Veiculo
{
    public Guid Id { get; set; }
    public TipoVeiculo Tipo { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public int Ano { get; set; }
    public decimal Quilometragem { get; set; }
    public decimal Valor { get; set; }
    public Guid ClienteId { get; set; }
    public Cliente ClienteResponsavel { get; set; } = null!;
}
