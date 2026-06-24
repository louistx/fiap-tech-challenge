using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Abstractions.Auth
{
    public interface ICurrentUser
    {
        Guid? UsuarioId { get; }
        Guid? FuncionarioId { get; }
        Guid? SessaoId { get; }            // claim "sid" — sessão (cadeia do refresh token)
        eTipoUsuario? TipoUsuario { get; }
        bool EstaAutenticado { get; }
    }
}
