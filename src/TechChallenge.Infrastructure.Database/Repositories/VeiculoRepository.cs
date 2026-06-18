using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class VeiculoRepository : Repository<Veiculo>, IVeiculoRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public VeiculoRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        #endregion

        #region Members of IVeiculoRepository

        public async Task<Veiculo?> GetByPlacaAsync(string placa)
        {
            return await _context.Veiculo.FirstOrDefaultAsync(v => v.Placa == placa);
        }

        #endregion
    }
}