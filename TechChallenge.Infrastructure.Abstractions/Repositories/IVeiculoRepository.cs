using TechChallenge.Domain.Entities;

namespace TechChallenge.Infrastructure.Abstractions.Repositories
{
    public interface IVeiculoRepository : IRepository<Veiculo>
    {
        Task<Veiculo?> GetByPlacaAsync(string placa);
    }
}