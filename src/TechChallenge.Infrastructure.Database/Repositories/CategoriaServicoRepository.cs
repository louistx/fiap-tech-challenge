using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class CategoriaServicoRepository : Repository<CategoriaServico>, ICategoriaServicoRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public CategoriaServicoRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        #endregion
    }
}