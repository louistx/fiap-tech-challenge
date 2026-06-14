using TechChallenge.Infrastructure.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Members of IClienteRepository

        #endregion
    }
}