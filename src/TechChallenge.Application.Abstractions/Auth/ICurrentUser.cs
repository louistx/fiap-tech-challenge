using System;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Abstractions.Auth
{
    public interface ICurrentUser
    {
        Guid? UsuarioId { get; }
        Guid? FuncionarioId { get; }
        eTipoUsuario? TipoUsuario { get; }
        bool EstaAutenticado { get; }
    }
}
