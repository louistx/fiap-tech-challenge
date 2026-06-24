using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Infrastructure.Auth;
public class CurrentUser : ICurrentUser
{
    private readonly ClaimsPrincipal? _principal;

    public CurrentUser(IHttpContextAccessor accessor)
    {
        _principal = accessor.HttpContext?.User;
    }

    public bool EstaAutenticado => _principal?.Identity?.IsAuthenticated ?? false;

    public Guid? UsuarioId => LerGuid(TokenService.ClaimSub);

    public Guid? FuncionarioId => LerGuid(TokenService.ClaimFuncionarioId);

    public Guid? SessaoId => LerGuid(TokenService.ClaimSessaoId);

    public eTipoUsuario? TipoUsuario =>
        Enum.TryParse<eTipoUsuario>(_principal?.FindFirstValue(TokenService.ClaimRole), out var tipo)
            ? tipo
            : null;

    private Guid? LerGuid(string claim) =>
        Guid.TryParse(_principal?.FindFirstValue(claim), out var valor) ? valor : null;
}
