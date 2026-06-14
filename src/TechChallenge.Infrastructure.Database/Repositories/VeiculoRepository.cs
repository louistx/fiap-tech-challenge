using TechChallenge.Infrastructure.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class VeiculoRepository : IVeiculoRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public VeiculoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Members of IVeiculoRepository

        #endregion
    }
}