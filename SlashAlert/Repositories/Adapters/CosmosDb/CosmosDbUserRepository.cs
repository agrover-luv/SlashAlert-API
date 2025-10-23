using Microsoft.Azure.Cosmos;
using SlashAlert.Repositories.Interfaces;
using UserModel = SlashAlert.Models.User;

namespace SlashAlert.Repositories.Adapters.CosmosDb
{
    public class CosmosDbUserRepository : BaseCosmosDbRepository<UserModel>, IUserRepository
    {
        public CosmosDbUserRepository(Container container) : base(container, "/partitionKey") { }

        public async Task<UserModel?> GetByEmailAsync(string email)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.email = @email")
                .WithParameter("@email", email);
            return await ExecuteSingleQueryAsync(queryDefinition);
        }

        public async Task<UserModel?> GetBySubAsync(string sub)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.sub = @sub")
                .WithParameter("@sub", sub);
            return await ExecuteSingleQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<UserModel>> GetByProviderAsync(string provider)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.provider = @provider")
                .WithParameter("@provider", provider);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<UserModel>> GetActiveUsersAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.is_active = @isActive")
                .WithParameter("@isActive", true);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<UserModel>> GetRecentLoginsAsync(int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.last_login >= @cutoffDate")
                .WithParameter("@cutoffDate", cutoffDate);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<UserModel?> GetByPartitionKeyAsync(string id, string partitionKey)
        {
            try
            {
                var response = await _container.ReadItemAsync<UserModel>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}