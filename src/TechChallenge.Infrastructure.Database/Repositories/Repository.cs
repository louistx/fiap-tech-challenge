using System;
using Microsoft.EntityFrameworkCore;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Properties

        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        #endregion

        #region Constructor

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        #endregion

        #region Members of IRepository

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                var entityEntry = _context.Entry(entity);
                var primaryKey = entityEntry.Metadata.FindPrimaryKey();
                var trackedEntry = primaryKey is null
                    ? null
                    : _context.ChangeTracker.Entries<T>().SingleOrDefault(entry =>
                        primaryKey.Properties.All(property =>
                            Equals(entry.Property(property.Name).CurrentValue,
                                entityEntry.Property(property.Name).CurrentValue)));

                if (trackedEntry is null)
                    _dbSet.Update(entity);
                else
                    trackedEntry.CurrentValues.SetValues(entity);
            }

            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
