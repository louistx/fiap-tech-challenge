using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class CategoriaProdutoRepository : Repository<CategoriaProduto>, ICategoriaProdutoRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public CategoriaProdutoRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        #endregion
    }
}