using MongoDB.Driver;
using MongoDB.Bson;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;

namespace SlashAlert.Repositories.Adapters.MongoDb
{
    /// <summary>
    /// Base repository for MongoDB operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public abstract class BaseMongoDbRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;
        protected readonly IMongoDbService _mongoDbService;

        protected BaseMongoDbRepository(IMongoDbService mongoDbService, string collectionName)
        {
            _mongoDbService = mongoDbService;
            _collection = mongoDbService.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Gets all entities from the collection filtered by created_by
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync(string createdBy)
        {
            try
            {
                var filter = Builders<T>.Filter.Eq(x => x.CreatedBy, createdBy);
                var result = await _collection.FindAsync(filter);
                var count = await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty);
                return await result.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving all entities from {_collection.CollectionNamespace.CollectionName}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets an entity by ID filtered by created_by
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(string id, string createdBy)
        {
            try
            {
                var filter = Builders<T>.Filter.And(
                    Builders<T>.Filter.Eq(x => x.Id, id),
                    Builders<T>.Filter.Eq(x => x.CreatedBy, createdBy)
                );
                var result = await _collection.FindAsync(filter);
                return await result.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving entity with ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a new entity
        /// </summary>
        public virtual async Task<T> CreateAsync(T entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = ObjectId.GenerateNewId().ToString();
                }
                entity.CreatedDate = DateTime.UtcNow;
                entity.UpdatedDate = DateTime.UtcNow;
                
                // Note: CreatedBy should be set by the calling controller before creating the entity

                await _collection.InsertOneAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error creating entity: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            try
            {
                entity.UpdatedDate = DateTime.UtcNow;
                
                var filter = Builders<T>.Filter.And(
                    Builders<T>.Filter.Eq(x => x.Id, entity.Id),
                    Builders<T>.Filter.Eq(x => x.CreatedBy, entity.CreatedBy)
                );
                var result = await _collection.ReplaceOneAsync(filter, entity);
                
                if (result.MatchedCount == 0)
                {
                    throw new InvalidOperationException($"Entity with ID {entity.Id} not found for update or user not authorized");
                }
                
                return entity;
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                throw new InvalidOperationException($"Error updating entity with ID {entity.Id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes an entity by ID filtered by created_by
        /// </summary>
        public virtual async Task<bool> DeleteAsync(string id, string createdBy)
        {
            try
            {
                var filter = Builders<T>.Filter.And(
                    Builders<T>.Filter.Eq(x => x.Id, id),
                    Builders<T>.Filter.Eq(x => x.CreatedBy, createdBy)
                );
                var result = await _collection.DeleteOneAsync(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting entity with ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if an entity exists by ID filtered by created_by
        /// </summary>
        public virtual async Task<bool> ExistsAsync(string id, string createdBy)
        {
            try
            {
                var filter = Builders<T>.Filter.And(
                    Builders<T>.Filter.Eq(x => x.Id, id),
                    Builders<T>.Filter.Eq(x => x.CreatedBy, createdBy)
                );
                var count = await _collection.CountDocumentsAsync(filter);
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error checking entity existence with ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets count of entities in the collection filtered by created_by
        /// </summary>
        public virtual async Task<int> CountAsync(string createdBy)
        {
            try
            {
                var filter = Builders<T>.Filter.Eq(x => x.CreatedBy, createdBy);
                var count = await _collection.CountDocumentsAsync(filter);
                return (int)count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error counting entities: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Helper method to execute a filter query with created_by filtering
        /// </summary>
        protected async Task<IEnumerable<T>> ExecuteFilterAsync(FilterDefinition<T> filter, string createdBy)
        {
            try
            {
                var combinedFilter = Builders<T>.Filter.And(
                    filter,
                    Builders<T>.Filter.Eq(x => x.CreatedBy, createdBy)
                );
                var result = await _collection.FindAsync(combinedFilter);
                return await result.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error executing filter query: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Helper method to execute a filter query and return a single result with created_by filtering
        /// </summary>
        protected async Task<T?> ExecuteSingleFilterAsync(FilterDefinition<T> filter, string createdBy)
        {
            try
            {
                var combinedFilter = Builders<T>.Filter.And(
                    filter,
                    Builders<T>.Filter.Eq(x => x.CreatedBy, createdBy)
                );
                var result = await _collection.FindAsync(combinedFilter);
                return await result.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error executing single filter query: {ex.Message}", ex);
            }
        }
    }
}