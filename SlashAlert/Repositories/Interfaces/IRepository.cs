namespace SlashAlert.Repositories.Interfaces
{
    /// <summary>
    /// Generic repository interface for basic CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string createdBy);
        Task<T?> GetByIdAsync(string id, string createdBy);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(string id, string createdBy);
        Task<bool> ExistsAsync(string id, string createdBy);
        Task<int> CountAsync(string createdBy);
    }
}