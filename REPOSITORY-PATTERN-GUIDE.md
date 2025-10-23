# Repository Pattern Implementation with Database Adapter

This implementation provides a flexible data access layer using the Repository pattern with adapters, allowing you to easily switch between different database providers (CSV, Cosmos DB, SQL) at runtime.

## Architecture Overview

The implementation follows these patterns:
- **Repository Pattern**: Provides a uniform interface for data access
- **Adapter Pattern**: Allows different database implementations behind the same interface
- **Dependency Injection**: Enables runtime configuration and testing flexibility
- **Factory Pattern**: Creates appropriate repository instances based on configuration

## Project Structure

```
SlashAlert/
├── Models/
│   ├── BaseEntity.cs              # Base class for all entities
│   ├── Alert.cs, Product.cs, etc. # Domain models
│   └── DatabaseSettings.cs        # Configuration models
├── Repositories/
│   ├── Interfaces/
│   │   ├── IRepository<T>.cs       # Generic repository interface
│   │   └── I*Repository.cs         # Specific entity interfaces
│   ├── Adapters/
│   │   ├── Csv/                    # CSV implementations
│   │   ├── CosmosDb/               # Cosmos DB implementations
│   │   └── Sql/                    # SQL implementations (placeholder)
│   └── RepositoryFactory.cs       # Factory for creating repositories
└── Controllers/                   # API controllers using repositories
```

## Configuration

### appsettings.json

```json
{
  "Database": {
    "Provider": "CSV",  // Options: "CSV", "CosmosDB", "SQL"
    "Csv": {
      "DatabasePath": "Database"
    },
    "CosmosDb": {
      "EndpointUri": "your-cosmos-endpoint",
      "PrimaryKey": "your-cosmos-key",
      "DatabaseName": "SlashAlert",
      "ContainerName": "Items"
    },
    "Sql": {
      "ConnectionString": "your-sql-connection-string",
      "Provider": "SqlServer"
    }
  }
}
```

## Usage Examples

### Switching Database Providers

To switch from CSV to Cosmos DB, simply change the configuration:

```json
{
  "Database": {
    "Provider": "CosmosDB"
  }
}
```

No code changes required! The factory will automatically create the appropriate repository implementations.

### Using Repositories in Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveProducts()
    {
        var products = await _productRepository.GetActiveProductsAsync();
        return Ok(products);
    }
}
```

### Available Repositories

Each repository provides both generic CRUD operations and entity-specific queries:

- **IAlertRepository**: Alert management with filtering by product, user, type
- **IProductRepository**: Product catalog with filtering by retailer, category, price
- **IRetailerRepository**: Retailer information with price guarantee filtering
- **IReviewRepository**: Product reviews with rating and verification filtering
- **IPriceHistoryRepository**: Price tracking with date range and trend analysis
- **IPriceCacheRepository**: Cached price data with URL and discount filtering
- **IUserRepository**: User management with provider and activity filtering

## API Endpoints

All controllers now provide rich filtering capabilities:

### Products API
- `GET /api/Product` - Get all products
- `GET /api/Product/{id}` - Get product by ID
- `GET /api/Product/retailer/{retailer}` - Filter by retailer
- `GET /api/Product/category/{category}` - Filter by category
- `GET /api/Product/active` - Get only active products
- `GET /api/Product/price-range?minPrice=10&maxPrice=100` - Filter by price range

### Alerts API
- `GET /api/Alert` - Get all alerts
- `GET /api/Alert/user/{userId}` - Get alerts for specific user
- `GET /api/Alert/product/{productId}` - Get alerts for specific product
- `GET /api/Alert/type/{alertType}` - Filter by alert type
- `GET /api/Alert/sent` - Get only sent alerts
- `GET /api/Alert/recent?days=7` - Get recent alerts

## Adding New Database Providers

To add a new database provider (e.g., MongoDB):

1. Create adapter classes in `Repositories/Adapters/MongoDB/`
2. Implement the base repository and specific repositories
3. Update `RepositoryFactory.cs` to handle the new provider
4. Add configuration section in `DatabaseSettings.cs`

## Testing

The repository pattern makes testing easier by allowing mock repositories:

```csharp
[Test]
public async Task GetActiveProducts_ReturnsOnlyActiveProducts()
{
    // Arrange
    var mockRepo = new Mock<IProductRepository>();
    mockRepo.Setup(r => r.GetActiveProductsAsync())
           .ReturnsAsync(GetTestActiveProducts());
    
    var controller = new ProductController(mockRepo.Object);
    
    // Act
    var result = await controller.GetActiveProducts();
    
    // Assert
    Assert.IsType<OkObjectResult>(result);
}
```

## Benefits

1. **Flexibility**: Switch database providers without changing business logic
2. **Testability**: Easy to mock repositories for unit testing
3. **Maintainability**: Clear separation of concerns
4. **Scalability**: Add new data sources easily
5. **Performance**: Can optimize different adapters for their specific strengths

## Environment Variables

For production deployments, you can override configuration using environment variables:

- `Database__Provider` - Database provider
- `COSMOS_DB_ENDPOINT` - Cosmos DB endpoint
- `COSMOS_DB_PRIMARY_KEY` - Cosmos DB key

## Migration Path

The implementation maintains backward compatibility:
- Existing CSV data continues to work
- Original CosmosDbService is still available for legacy endpoints
- Controllers now provide enhanced querying capabilities

This allows for gradual migration while maintaining system stability.