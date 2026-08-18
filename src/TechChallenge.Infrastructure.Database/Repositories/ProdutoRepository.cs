using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public ProdutoRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        #endregion

        #region Member of IProdutoRepository

        public async Task<bool> ExisteProdutoComCategoria(Guid categoriaId)
        {
            return await _context.Produto
                .AnyAsync(p => p.CategoriaId == categoriaId);
        }

        #endregion
    }
}