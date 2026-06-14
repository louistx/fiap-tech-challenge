using TechChallenge.Infrastructure.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class FuncionarioRepository : IFuncionarioRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public FuncionarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Members of IFuncionarioRepository

        #endregion
    }
}