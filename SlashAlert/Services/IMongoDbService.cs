using MongoDB.Driver;

namespace SlashAlert.Services
{
    /// <summary>
    /// Service for managing MongoDB connections and database operations
    /// </summary>
    public interface IMongoDbService
    {
        /// <summary>
        /// Gets the MongoDB database instance
        /// </summary>
        IMongoDatabase Database { get; }

        /// <summary>
        /// Gets a specific collection from the database
        /// </summary>
        /// <typeparam name="T">The document type</typeparam>
        /// <param name="collectionName">Name of the collection</param>
        /// <returns>MongoDB collection</returns>
        IMongoCollection<T> GetCollection<T>(string collectionName);

        /// <summary>
        /// Tests the database connection
        /// </summary>
        /// <returns>True if connection is successful</returns>
        Task<bool> TestConnectionAsync();
    }
}