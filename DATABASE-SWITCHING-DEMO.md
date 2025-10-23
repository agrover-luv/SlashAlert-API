# Database Provider Switching Demo

This demonstrates how easy it is to switch between different database providers.

## Current Configuration (CSV)

In `appsettings.json`:
```json
{
  "Database": {
    "Provider": "CSV"
  }
}
```

## Switch to Cosmos DB

Simply change the configuration:
```json
{
  "Database": {
    "Provider": "CosmosDB",
    "CosmosDb": {
      "EndpointUri": "https://your-cosmos-account.documents.azure.com:443/",
      "PrimaryKey": "your-primary-key",
      "DatabaseName": "SlashAlert",
      "ContainerName": "Items"
    }
  }
}
```

## Testing Different Providers

### Using CSV (Default)
```bash
# Start the application
dotnet run

# Test CSV data access
curl http://localhost:5000/api/Product
curl http://localhost:5000/api/Alert
```

### Using Cosmos DB
```bash
# Update appsettings.json to use CosmosDB
# Restart the application
dotnet run

# Same endpoints, different data source!
curl http://localhost:5000/api/Product
curl http://localhost:5000/api/Alert
```

## Environment Variable Override

For production or CI/CD:
```bash
export Database__Provider="CosmosDB"
export COSMOS_DB_ENDPOINT="https://your-account.documents.azure.com:443/"
export COSMOS_DB_PRIMARY_KEY="your-key"

dotnet run
```

## Advanced Filtering Examples

Thanks to the repository pattern, all providers support the same rich querying:

### Products
```bash
# Get products by retailer
curl http://localhost:5000/api/Product/retailer/amazon

# Get products by category
curl http://localhost:5000/api/Product/category/electronics

# Get products in price range
curl "http://localhost:5000/api/Product/price-range?minPrice=100&maxPrice=500"

# Get active products only
curl http://localhost:5000/api/Product/active
```

### Alerts
```bash
# Get alerts for specific user
curl http://localhost:5000/api/Alert/user/6876aec99255670e28843752

# Get price drop alerts only
curl http://localhost:5000/api/Alert/type/price_drop

# Get recent alerts (last 7 days)
curl "http://localhost:5000/api/Alert/recent?days=7"

# Get sent alerts only
curl http://localhost:5000/api/AlertExport/sent
```

### Reviews
```bash
# Get 5-star reviews
curl http://localhost:5000/api/ReviewExport/rating/5

# Get verified reviews only
curl http://localhost:5000/api/ReviewExport/verified

# Get top-rated reviews (4+ stars)
curl "http://localhost:5000/api/ReviewExport/top-rated?minRating=4"
```

The beauty of this implementation is that **all these endpoints work identically regardless of whether you're using CSV, Cosmos DB, or SQL as your data source!**