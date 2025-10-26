using Microsoft.Azure.Cosmos;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.CosmosDb
{
    public class CosmosDbProductRepository : BaseCosmosDbRepository<Product>, IProductRepository
    {
        public CosmosDbProductRepository(Container container) : base(container) { }

        public async Task<IEnumerable<Product>> GetByRetailerAsync(string retailer, string createdBy)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE LOWER(c.retailer) = LOWER(@retailer) AND c.created_by = @createdBy")
                .WithParameter("@retailer", retailer)
                .WithParameter("@createdBy", createdBy);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category, string createdBy)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE LOWER(c.category) = LOWER(@category) AND c.created_by = @createdBy")
                .WithParameter("@category", category)
                .WithParameter("@createdBy", createdBy);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync(string createdBy)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE (c.is_active = @isActive OR NOT IS_DEFINED(c.is_active)) AND c.created_by = @createdBy")
                .WithParameter("@isActive", "true")
                .WithParameter("@createdBy", createdBy);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Product>> GetByCreatedByAsync(string createdBy)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.created_by = @createdBy")
                .WithParameter("@createdBy", createdBy);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, string createdBy)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE IS_NUMBER(c.current_price) AND c.current_price >= @minPrice AND c.current_price <= @maxPrice AND c.created_by = @createdBy")
                .WithParameter("@minPrice", minPrice)
                .WithParameter("@maxPrice", maxPrice)
                .WithParameter("@createdBy", createdBy);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<Product?> GetByUrlAsync(string url, string createdBy)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.url = @url AND c.created_by = @createdBy")
                .WithParameter("@url", url)
                .WithParameter("@createdBy", createdBy);
            return await ExecuteSingleQueryAsync(queryDefinition);
        }
    }
}