using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Auth.Sessoes;

public class RevogarSessaoService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUser _currentUser;

    public RevogarSessaoService(IRefreshTokenRepository refreshTokenRepository, ICurrentUser currentUser)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
    }

    public void RevogarSessao(Guid sessaoId)
    {
        if (_currentUser.UsuarioId is not { } usuarioId)
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        var sessao = _refreshTokenRepository.GetSessaoAsync(sessaoId).GetAwaiter().GetResult();
        if (sessao.Count == 0 || sessao.Any(t => t.UsuarioId != usuarioId))
            throw new KeyNotFoundException("Sessão não encontrada.");

        _refreshTokenRepository
            .RevogarSessaoAsync(sessaoId, "logout", DateTime.UtcNow)
            .GetAwaiter().GetResult();
    }
}
