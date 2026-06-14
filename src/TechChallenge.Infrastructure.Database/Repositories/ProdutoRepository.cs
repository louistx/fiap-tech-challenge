using TechChallenge.Infrastructure.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public ProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Members of IProdutoRepository

        #endregion
    }
}