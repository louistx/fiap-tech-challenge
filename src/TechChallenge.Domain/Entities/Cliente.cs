using System;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Entities;

public class Cliente
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoDocumento TipoDocumento { get; set; }
    public string Documento { get; set; } = string.Empty;
    public Guid EnderecoId { get; set; }
    public Endereco Endereco { get; set; } = null!;
}
