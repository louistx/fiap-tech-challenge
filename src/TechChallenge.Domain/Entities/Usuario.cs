using System;
using System.Collections.Generic;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public eTipoUsuario TipoUsuario { get; set; }
    public bool Ativo { get; set; } = true;

    // Vínculo opcional com Funcionario (regra de dono-do-recurso). Nullable: Usuario pode existir sem Funcionario.
    public Guid? FuncionarioId { get; set; }
    public Funcionario? Funcionario { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
