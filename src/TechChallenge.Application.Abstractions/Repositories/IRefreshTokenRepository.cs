using TechChallenge.Domain.Entities;

namespace TechChallenge.Application.Abstractions.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByHashAsync(string hash);                 // + Include Usuario
        Task<List<RefreshToken>> GetSessaoAsync(Guid sessaoId);
        Task<List<RefreshToken>> GetAtivasDoUsuarioAsync(Guid usuarioId, DateTime agora);
        Task RevogarSessaoAsync(Guid sessaoId, string motivo, DateTime agora);
        Task RevogarTodasDoUsuarioAsync(Guid usuarioId, string motivo, DateTime agora);
    }
}
