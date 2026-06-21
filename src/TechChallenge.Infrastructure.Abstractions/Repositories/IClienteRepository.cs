using TechChallenge.Domain.Entities;

namespace TechChallenge.Infrastructure.Abstractions.Repositories
{
    public interface IClienteRepository
    {
        Cliente GetByDocument(string document);
    }
}