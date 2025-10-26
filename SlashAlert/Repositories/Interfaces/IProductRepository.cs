using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByRetailerAsync(string retailer, string createdBy);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category, string createdBy);
        Task<IEnumerable<Product>> GetActiveProductsAsync(string createdBy);
        Task<IEnumerable<Product>> GetByCreatedByAsync(string createdBy);
        Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, string createdBy);
        Task<Product?> GetByUrlAsync(string url, string createdBy);
    }
}