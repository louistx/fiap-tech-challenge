using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entities;
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

        public Cliente GetByDocument(string document)
        {
            Cliente entity = _context.Cliente.FirstOrDefaultAsync(c => c.Cpf.Equals(document)).Result;

            if (entity is null) throw new Exception();

            return entity;            
        }

        #endregion
    }
}