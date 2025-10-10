using Microsoft.Azure.Cosmos;
using SlashAlert.Models;
using Microsoft.Extensions.Options;
using System.Net;
using UserModel = SlashAlert.Models.User;

namespace SlashAlert.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly Container _container;

        public CosmosDbService(CosmosClient cosmosClient, IOptions<CosmosDbSettings> settings)
        {
            var cosmosDbSettings = settings.Value;
            _container = cosmosClient.GetContainer(cosmosDbSettings.DatabaseName, cosmosDbSettings.ContainerName);
        }

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            try
            {
                var query = _container.GetItemQueryIterator<UserModel>(new QueryDefinition("SELECT * FROM c"));
                var results = new List<UserModel>();

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    results.AddRange(response.ToList());
                }

                return results;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<UserModel>();
            }
        }

        public async Task<UserModel?> GetUserByIdAsync(string id, string partitionKey)
        {
            try
            {
                var response = await _container.ReadItemAsync<UserModel>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<UserModel?> GetUserBySubAsync(string sub)
        {
            try
            {
                var query = _container.GetItemQueryIterator<UserModel>(
                    new QueryDefinition("SELECT * FROM c WHERE c.sub = @sub")
                        .WithParameter("@sub", sub));

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    return response.FirstOrDefault();
                }

                return null;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<UserModel?> GetUserByEmailAsync(string email)
        {
            try
            {
                var query = _container.GetItemQueryIterator<UserModel>(
                    new QueryDefinition("SELECT * FROM c WHERE c.email = @email")
                        .WithParameter("@email", email));

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    return response.FirstOrDefault();
                }

                return null;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<UserModel>> GetUsersByProviderAsync(string provider)
        {
            try
            {
                var query = _container.GetItemQueryIterator<UserModel>(
                    new QueryDefinition("SELECT * FROM c WHERE c.provider = @provider")
                        .WithParameter("@provider", provider));

                var results = new List<UserModel>();

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    results.AddRange(response.ToList());
                }

                return results;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<UserModel>();
            }
        }

        public async Task<IEnumerable<UserModel>> GetActiveUsersAsync()
        {
            try
            {
                var query = _container.GetItemQueryIterator<UserModel>(
                    new QueryDefinition("SELECT * FROM c WHERE c.is_active = @isActive")
                        .WithParameter("@isActive", true));

                var results = new List<UserModel>();

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    results.AddRange(response.ToList());
                }

                return results;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<UserModel>();
            }
        }

        public async Task<IEnumerable<UserModel>> GetRecentLoginsAsync(int days = 30)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-days);
                var query = _container.GetItemQueryIterator<UserModel>(
                    new QueryDefinition("SELECT * FROM c WHERE c.last_login >= @cutoffDate")
                        .WithParameter("@cutoffDate", cutoffDate));

                var results = new List<UserModel>();

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    results.AddRange(response.ToList());
                }

                return results;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<UserModel>();
            }
        }
    }
}