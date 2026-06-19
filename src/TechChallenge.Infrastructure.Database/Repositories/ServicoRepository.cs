using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class ServicoRepository : Repository<Servico>, IServicoRepository
    {
        public ServicoRepository(ApplicationDbContext context) : base(context) { }
    }
}