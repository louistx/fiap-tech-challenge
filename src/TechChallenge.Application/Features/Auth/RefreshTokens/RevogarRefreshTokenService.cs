using System;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Auth.RefreshTokens;

public class RevogarRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUser _currentUser;

    public RevogarRefreshTokenService(IRefreshTokenRepository refreshTokenRepository, ICurrentUser currentUser)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
    }

    public void RevogarRefreshToken(Guid refreshTokenId)
    {
        if (_currentUser.UsuarioId is not { } usuarioId)
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        var token = _refreshTokenRepository.GetByIdAsync(refreshTokenId).GetAwaiter().GetResult();
        if (token is null || token.UsuarioId != usuarioId)
            throw new KeyNotFoundException("Refresh token não encontrado.");

        token.RevogadoEm = DateTime.UtcNow;
        _refreshTokenRepository.UpdateAsync(token).GetAwaiter().GetResult();
    }
}
