using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Abstractions.Repositories
{
    public interface IOrdemServicoRepository : IRepository<OrdemServico>
    {
        Task<List<OrdemServico>> GetByStatusAsync(eStatusOS status);
        Task<OrdemServico?> GetOSAtivaMecanicoAsync(Guid mecanicoId);
    }
}