using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.Sql
{
    public abstract class BaseSqlRepository<T> : IRepository<T> where T : BaseEntity
    {
        // TODO: Add SQL connection string and DbContext
        // private readonly string _connectionString;
        // private readonly DbContext _context;

        protected BaseSqlRepository()
        {
            // TODO: Initialize SQL connection
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            // TODO: Implement SQL query to get all entities
            throw new NotImplementedException("SQL repository not yet implemented");
        }

        public virtual async Task<T?> GetByIdAsync(string id)
        {
            // TODO: Implement SQL query to get entity by ID
            throw new NotImplementedException("SQL repository not yet implemented");
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            // TODO: Implement SQL insert
            throw new NotImplementedException("SQL repository not yet implemented");
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            // TODO: Implement SQL update
            throw new NotImplementedException("SQL repository not yet implemented");
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            // TODO: Implement SQL delete
            throw new NotImplementedException("SQL repository not yet implemented");
        }

        public virtual async Task<bool> ExistsAsync(string id)
        {
            // TODO: Implement SQL exists check
            throw new NotImplementedException("SQL repository not yet implemented");
        }

        public virtual async Task<int> CountAsync()
        {
            // TODO: Implement SQL count
            throw new NotImplementedException("SQL repository not yet implemented");
        }
    }
}