using System;
namespace TechChallenge.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;   // SHA-256 do valor cru (256-bit)
    public Guid SessaoId { get; set; }                       // todos os tokens da mesma cadeia de login (sessão)

    public DateTime CriadoEm { get; set; }
    public DateTime ExpiraEm { get; set; }                   // sliding: emissão + RefreshTokenDays
    public DateTime SessaoExpiraEm { get; set; }             // absoluto: fixado no login (now + MaxDays)

    public DateTime? RevogadoEm { get; set; }
    public Guid? SubstituidoPorId { get; set; }              // token que o sucedeu na rotação
    public string? MotivoRevogacao { get; set; }             // rotacionado | logout | reuso-detectado | usuario-desativado | reset-senha

    public string? UserAgent { get; set; }
    public string? IpCriacao { get; set; }

    public bool EstaAtivo(DateTime agora) =>
        RevogadoEm is null
        && SubstituidoPorId is null
        && agora < ExpiraEm
        && agora < SessaoExpiraEm;
}
