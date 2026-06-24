using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Auth.Logout;

public class LogoutService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUser _currentUser;

    public LogoutService(
        IRefreshTokenRepository refreshTokenRepository,
        ICurrentUser currentUser)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
    }

    // Revoga a sessão atual (claim "sid" do access token).
    public void Logout()
    {
        if (_currentUser.SessaoId is not { } sessaoId)
            throw new UnauthorizedAccessException("Sessão não identificada.");

        _refreshTokenRepository
            .RevogarSessaoAsync(sessaoId, "logout", DateTime.UtcNow)
            .GetAwaiter().GetResult();
    }

    // Revoga todas as sessões do usuário autenticado.
    public void LogoutTodas()
    {
        if (_currentUser.UsuarioId is not { } usuarioId)
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        _refreshTokenRepository
            .RevogarTodasDoUsuarioAsync(usuarioId, "logout-all", DateTime.UtcNow)
            .GetAwaiter().GetResult();
    }
}
