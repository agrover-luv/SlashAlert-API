using Microsoft.Azure.Cosmos;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using System.Net;

namespace SlashAlert.Repositories.Adapters.CosmosDb
{
    public abstract class BaseCosmosDbRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly Container _container;
        protected readonly string _partitionKeyPath;

        protected BaseCosmosDbRepository(Container container, string partitionKeyPath = "/partitionKey")
        {
            _container = container;
            _partitionKeyPath = partitionKeyPath;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var query = _container.GetItemQueryIterator<T>(new QueryDefinition("SELECT * FROM c"));
                var results = new List<T>();

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    results.AddRange(response.ToList());
                }

                return results;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<T>();
            }
        }

        public virtual async Task<T?> GetByIdAsync(string id)
        {
            try
            {
                // For entities that don't have a specific partition key, we'll query by id
                var query = _container.GetItemQueryIterator<T>(
                    new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                        .WithParameter("@id", id));

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

        public virtual async Task<T> CreateAsync(T entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = Guid.NewGuid().ToString("N")[..24]; // MongoDB-like ID
                }
                entity.CreatedDate = DateTime.UtcNow;
                entity.UpdatedDate = DateTime.UtcNow;

                var response = await _container.CreateItemAsync(entity);
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                throw new InvalidOperationException($"Error creating entity: {ex.Message}", ex);
            }
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            try
            {
                entity.UpdatedDate = DateTime.UtcNow;
                
                var response = await _container.ReplaceItemAsync(entity, entity.Id);
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException($"Entity with ID {entity.Id} not found for update", ex);
            }
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            try
            {
                // For simplicity, we'll query first to get the partition key
                var entity = await GetByIdAsync(id);
                if (entity == null) return false;

                await _container.DeleteItemAsync<T>(id, PartitionKey.None);
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public virtual async Task<bool> ExistsAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public virtual async Task<int> CountAsync()
        {
            try
            {
                var query = _container.GetItemQueryIterator<int>(
                    new QueryDefinition("SELECT VALUE COUNT(1) FROM c"));

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    return response.FirstOrDefault();
                }

                return 0;
            }
            catch (CosmosException)
            {
                return 0;
            }
        }

        protected async Task<IEnumerable<T>> ExecuteQueryAsync(QueryDefinition queryDefinition)
        {
            try
            {
                var query = _container.GetItemQueryIterator<T>(queryDefinition);
                var results = new List<T>();

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    results.AddRange(response.ToList());
                }

                return results;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<T>();
            }
        }

        protected async Task<T?> ExecuteSingleQueryAsync(QueryDefinition queryDefinition)
        {
            try
            {
                var query = _container.GetItemQueryIterator<T>(queryDefinition);

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
    }
}