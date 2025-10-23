using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByRetailerAsync(string retailer);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> GetByCreatedByIdAsync(string createdById);
        Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<Product?> GetByUrlAsync(string url);
    }
}