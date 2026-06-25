using System;
namespace TechChallenge.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public DateTime ExpiraEm { get; set; }
    public DateTime? RevogadoEm { get; set; }

    public bool EstaAtivo(DateTime agora) =>
        RevogadoEm is null
        && agora < ExpiraEm;
}
