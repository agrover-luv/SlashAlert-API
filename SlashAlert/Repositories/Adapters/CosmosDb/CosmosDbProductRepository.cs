using Microsoft.Azure.Cosmos;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.CosmosDb
{
    public class CosmosDbProductRepository : BaseCosmosDbRepository<Product>, IProductRepository
    {
        public CosmosDbProductRepository(Container container) : base(container) { }

        public async Task<IEnumerable<Product>> GetByRetailerAsync(string retailer)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE LOWER(c.retailer) = LOWER(@retailer)")
                .WithParameter("@retailer", retailer);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE LOWER(c.category) = LOWER(@category)")
                .WithParameter("@category", category);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.is_active = @isActive OR NOT IS_DEFINED(c.is_active)")
                .WithParameter("@isActive", "true");
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Product>> GetByCreatedByIdAsync(string createdById)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.created_by_id = @createdById")
                .WithParameter("@createdById", createdById);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE IS_NUMBER(c.current_price) AND c.current_price >= @minPrice AND c.current_price <= @maxPrice")
                .WithParameter("@minPrice", minPrice)
                .WithParameter("@maxPrice", maxPrice);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<Product?> GetByUrlAsync(string url)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.url = @url")
                .WithParameter("@url", url);
            return await ExecuteSingleQueryAsync(queryDefinition);
        }
    }
}