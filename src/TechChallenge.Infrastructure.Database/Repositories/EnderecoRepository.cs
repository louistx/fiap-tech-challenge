using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class EnderecoRepository : IEnderecoRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public EnderecoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Members of IEnderecoRepository

        #endregion
    }
}