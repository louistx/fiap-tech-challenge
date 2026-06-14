using TechChallenge.Infrastructure.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class ServicoRepository : IServicoRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public ServicoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Members of IServicoRepository

        #endregion
    }
}