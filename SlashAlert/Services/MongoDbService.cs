using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SlashAlert.Models;

namespace SlashAlert.Services
{
    /// <summary>
    /// Service for managing MongoDB connections and database operations
    /// </summary>
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;

        public MongoDbService(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoSettings = databaseSettings.Value.MongoDb;
            if (mongoSettings == null)
            {
                throw new InvalidOperationException("MongoDB settings not configured");
            }

            if (string.IsNullOrEmpty(mongoSettings.ConnectionString))
            {
                throw new InvalidOperationException("MongoDB connection string not configured");
            }

            _client = new MongoClient(mongoSettings.ConnectionString);
            _database = _client.GetDatabase(mongoSettings.DatabaseName);
        }

        /// <summary>
        /// Gets the MongoDB database instance
        /// </summary>
        public IMongoDatabase Database => _database;

        /// <summary>
        /// Gets a specific collection from the database
        /// </summary>
        /// <typeparam name="T">The document type</typeparam>
        /// <param name="collectionName">Name of the collection</param>
        /// <returns>MongoDB collection</returns>
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Tests the database connection
        /// </summary>
        /// <returns>True if connection is successful</returns>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                // Ping the database to test connectivity
                await _database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}