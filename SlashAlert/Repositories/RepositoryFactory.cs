using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories.Adapters.CosmosDb;
using SlashAlert.Repositories.Adapters.Csv;
using SlashAlert.Repositories.Adapters.MongoDb;
using SlashAlert.Repositories.Adapters.Sql;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;

namespace SlashAlert.Repositories
{
    public interface IRepositoryFactory
    {
        IAlertRepository CreateAlertRepository();
        IProductRepository CreateProductRepository();
        IRetailerRepository CreateRetailerRepository();
        IReviewRepository CreateReviewRepository();
        IPriceHistoryRepository CreatePriceHistoryRepository();
        IPriceCacheRepository CreatePriceCacheRepository();
        IUserRepository CreateUserRepository();
    }

    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly DatabaseSettings _databaseSettings;
        private readonly ICsvService _csvService;
        private readonly Container? _cosmosContainer;
        private readonly IMongoDbService? _mongoDbService;

        public RepositoryFactory(
            IOptions<DatabaseSettings> databaseSettings,
            ICsvService csvService,
            Container? cosmosContainer = null,
            IMongoDbService? mongoDbService = null)
        {
            _databaseSettings = databaseSettings.Value;
            _csvService = csvService;
            _cosmosContainer = cosmosContainer;
            _mongoDbService = mongoDbService;
        }

        public IAlertRepository CreateAlertRepository()
        {
            return _databaseSettings.Provider.ToUpper() switch
            {
                "CSV" => new CsvAlertRepository(_csvService),
                "COSMOSDB" => new CosmosDbAlertRepository(_cosmosContainer ?? throw new InvalidOperationException("Cosmos container not configured")),
                "MONGODB" => new MongoDbAlertRepository(_mongoDbService ?? throw new InvalidOperationException("MongoDB service not configured")),
                "SQL" => throw new NotImplementedException("SQL repository not yet implemented"),
                _ => throw new ArgumentException($"Unknown database provider: {_databaseSettings.Provider}")
            };
        }

        public IProductRepository CreateProductRepository()
        {
            return _databaseSettings.Provider.ToUpper() switch
            {
                "CSV" => new CsvProductRepository(_csvService),
                "COSMOSDB" => new CosmosDbProductRepository(_cosmosContainer ?? throw new InvalidOperationException("Cosmos container not configured")),
                "MONGODB" => new MongoDbProductRepository(_mongoDbService ?? throw new InvalidOperationException("MongoDB service not configured")),
                "SQL" => throw new NotImplementedException("SQL repository not yet implemented"),
                _ => throw new ArgumentException($"Unknown database provider: {_databaseSettings.Provider}")
            };
        }

        public IRetailerRepository CreateRetailerRepository()
        {
            return _databaseSettings.Provider.ToUpper() switch
            {
                "CSV" => new CsvRetailerRepository(_csvService),
                "COSMOSDB" => throw new NotImplementedException("CosmosDB retailer repository not yet implemented"),
                "MONGODB" => new MongoDbRetailerRepository(_mongoDbService ?? throw new InvalidOperationException("MongoDB service not configured")),
                "SQL" => throw new NotImplementedException("SQL repository not yet implemented"),
                _ => throw new ArgumentException($"Unknown database provider: {_databaseSettings.Provider}")
            };
        }

        public IReviewRepository CreateReviewRepository()
        {
            return _databaseSettings.Provider.ToUpper() switch
            {
                "CSV" => new CsvReviewRepository(_csvService),
                "COSMOSDB" => throw new NotImplementedException("CosmosDB review repository not yet implemented"),
                "MONGODB" => new MongoDbReviewRepository(_mongoDbService ?? throw new InvalidOperationException("MongoDB service not configured")),
                "SQL" => throw new NotImplementedException("SQL repository not yet implemented"),
                _ => throw new ArgumentException($"Unknown database provider: {_databaseSettings.Provider}")
            };
        }

        public IPriceHistoryRepository CreatePriceHistoryRepository()
        {
            return _databaseSettings.Provider.ToUpper() switch
            {
                "CSV" => new CsvPriceHistoryRepository(_csvService),
                "COSMOSDB" => throw new NotImplementedException("CosmosDB price history repository not yet implemented"),
                "MONGODB" => new MongoDbPriceHistoryRepository(_mongoDbService ?? throw new InvalidOperationException("MongoDB service not configured")),
                "SQL" => throw new NotImplementedException("SQL repository not yet implemented"),
                _ => throw new ArgumentException($"Unknown database provider: {_databaseSettings.Provider}")
            };
        }

        public IPriceCacheRepository CreatePriceCacheRepository()
        {
            return _databaseSettings.Provider.ToUpper() switch
            {
                "CSV" => new CsvPriceCacheRepository(_csvService),
                "COSMOSDB" => throw new NotImplementedException("CosmosDB price cache repository not yet implemented"),
                "MONGODB" => new MongoDbPriceCacheRepository(_mongoDbService ?? throw new InvalidOperationException("MongoDB service not configured")),
                "SQL" => throw new NotImplementedException("SQL repository not yet implemented"),
                _ => throw new ArgumentException($"Unknown database provider: {_databaseSettings.Provider}")
            };
        }

        public IUserRepository CreateUserRepository()
        {
            return _databaseSettings.Provider.ToUpper() switch
            {
                "CSV" => new CsvUserRepository(_csvService),
                "COSMOSDB" => new CosmosDbUserRepository(_cosmosContainer ?? throw new InvalidOperationException("Cosmos container not configured")),
                "MONGODB" => new MongoDbUserRepository(_mongoDbService ?? throw new InvalidOperationException("MongoDB service not configured")),
                "SQL" => new SqlUserRepository(),
                _ => throw new ArgumentException($"Unknown database provider: {_databaseSettings.Provider}")
            };
        }
    }
}