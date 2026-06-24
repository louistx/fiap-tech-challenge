using System;
namespace TechChallenge.Domain.Entities;

public class Cliente
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Rg { get; set; } = string.Empty;
    public Guid EnderecoId { get; set; }
    public Endereco Endereco { get; set; } = null!;
}
