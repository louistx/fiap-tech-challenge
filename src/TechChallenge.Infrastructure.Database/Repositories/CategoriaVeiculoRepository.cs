using System;
using System.Collections.Generic;
using System.Text;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class CategoriaVeiculoRepository : Repository<CategoriaVeiculo>, ICategoriaVeiculoRepository
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public CategoriaVeiculoRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        #endregion
    }
}