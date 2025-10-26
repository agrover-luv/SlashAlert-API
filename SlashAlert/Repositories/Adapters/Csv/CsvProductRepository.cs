using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.Csv
{
    public class CsvProductRepository : BaseCsvRepository<Product>, IProductRepository
    {
        public CsvProductRepository(ICsvService csvService) : base(csvService, "Product_export.csv") { }

        public async Task<IEnumerable<Product>> GetByRetailerAsync(string retailer, string createdBy)
        {
            var products = await GetAllAsync(createdBy);
            return products.Where(p => p.Retailer.Equals(retailer, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category, string createdBy)
        {
            var products = await GetAllAsync(createdBy);
            return products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync(string createdBy)
        {
            var products = await GetAllAsync(createdBy);
            return products.Where(p => p.IsActive == "true" || string.IsNullOrEmpty(p.IsActive));
        }

        public async Task<IEnumerable<Product>> GetByCreatedByAsync(string createdBy)
        {
            var products = await GetAllAsync(createdBy);
            return products.Where(p => p.CreatedBy == createdBy);
        }

        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, string createdBy)
        {
            var products = await GetAllAsync(createdBy);
            return products.Where(p => 
            {
                if (decimal.TryParse(p.CurrentPrice, out var currentPrice))
                {
                    return currentPrice >= minPrice && currentPrice <= maxPrice;
                }
                return false;
            });
        }

        public async Task<Product?> GetByUrlAsync(string url, string createdBy)
        {
            var products = await GetAllAsync(createdBy);
            return products.FirstOrDefault(p => p.Url == url);
        }
    }
}