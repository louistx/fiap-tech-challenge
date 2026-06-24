using System;
using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByHashAsync(string hash)
        {
            return await _context.RefreshToken
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.TokenHash == hash);
        }

        public async Task<List<RefreshToken>> GetSessaoAsync(Guid sessaoId)
        {
            return await _context.RefreshToken
                .Where(t => t.SessaoId == sessaoId)
                .ToListAsync();
        }

        public async Task<List<RefreshToken>> GetAtivasDoUsuarioAsync(Guid usuarioId, DateTime agora)
        {
            return await _context.RefreshToken
                .Where(t => t.UsuarioId == usuarioId
                            && t.RevogadoEm == null
                            && t.SubstituidoPorId == null
                            && agora < t.ExpiraEm
                            && agora < t.SessaoExpiraEm)
                .ToListAsync();
        }

        public async Task RevogarSessaoAsync(Guid sessaoId, string motivo, DateTime agora)
        {
            var tokens = await _context.RefreshToken
                .Where(t => t.SessaoId == sessaoId && t.RevogadoEm == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevogadoEm = agora;
                token.MotivoRevogacao = motivo;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RevogarTodasDoUsuarioAsync(Guid usuarioId, string motivo, DateTime agora)
        {
            var tokens = await _context.RefreshToken
                .Where(t => t.UsuarioId == usuarioId && t.RevogadoEm == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevogadoEm = agora;
                token.MotivoRevogacao = motivo;
            }

            await _context.SaveChangesAsync();
        }
    }
}
