using TechChallenge.Infrastructure.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class OrdemServicoRepository : IOrdemServicoRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public OrdemServicoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Members of IOrdemServicoRepository

        #endregion
    }
}