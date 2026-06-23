using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class OrdemServicoRepository : Repository<OrdemServico>, IOrdemServicoRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public OrdemServicoRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        #endregion

        #region Members of IOrdemServicoRepository

        public override async Task<OrdemServico?> GetByIdAsync(Guid id)
        {
            return await _context.OrdemServico
                .Include(os => os.Servicos)
                .Include(os => os.Produtos)
                .FirstOrDefaultAsync(os => os.Id == id);
        }

        public override async Task<List<OrdemServico>> GetAllAsync()
        {
            return await _context.OrdemServico
                .Include(os => os.Servicos)
                .Include(os => os.Produtos)
                .ToListAsync();
        }

        public async Task<List<OrdemServico>> GetByStatusAsync(eStatusOS status)
        {
            return await _context.OrdemServico
                .Where(os => os.Status == status)
                .Include(os => os.Veiculo)
                .Include(os => os.FuncionarioResponsavel)
                .Include(os => os.ClienteResponsavel)
                .Include(os => os.Servicos)
                .Include(os => os.Produtos)
                .ToListAsync();
        }

        public async Task<OrdemServico?> GetOSAtivaMecanicoAsync(Guid mecanicoId)
        {
            return await _context.OrdemServico
                .FirstOrDefaultAsync(os =>
                    os.FuncionarioResponsavelId == mecanicoId &&
                    os.Status == eStatusOS.EmDiagnostico);
        }

        #endregion
    }
}
