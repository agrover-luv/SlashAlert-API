using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;

namespace SlashAlert.Migration
{
    public class CsvToMongoMigrationService
    {
        private readonly ICsvService _csvService;
        private readonly IMongoDbService _mongoDbService;

        public CsvToMongoMigrationService(ICsvService csvService, IMongoDbService mongoDbService)
        {
            _csvService = csvService;
            _mongoDbService = mongoDbService;
        }

        public async Task<bool> MigrateAllDataAsync()
        {
            try
            {
                Console.WriteLine("🚀 Starting CSV to MongoDB migration...");

                // Migrate Products
                await MigrateProductsAsync();
                
                // Migrate Alerts
                await MigrateAlertsAsync();
                
                // Migrate Retailers
                await MigrateRetailersAsync();
                
                // Migrate Reviews
                await MigrateReviewsAsync();
                
                // Migrate PriceHistory
                await MigratePriceHistoryAsync();
                
                // Migrate PriceCache
                await MigratePriceCacheAsync();

                Console.WriteLine("✅ Migration completed successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Migration failed: {ex.Message}");
                return false;
            }
        }

        private async Task MigrateProductsAsync()
        {
            try
            {
                var products = await _csvService.GetAllAsync<Product>("Product_export.csv");
                var mongoCollection = _mongoDbService.GetCollection<Product>("product");

                var productList = products.ToList();
                if (productList.Any())
                {
                    await mongoCollection.InsertManyAsync(productList);
                    Console.WriteLine($"✅ Migrated {productList.Count} products");
                }
                else
                {
                    Console.WriteLine("⚪ No products found in CSV");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to migrate products: {ex.Message}");
            }
        }

        private async Task MigrateAlertsAsync()
        {
            try
            {
                var alerts = await _csvService.GetAllAsync<Alert>("Alert_export.csv");
                var mongoCollection = _mongoDbService.GetCollection<Alert>("alert");

                var alertList = alerts.ToList();
                if (alertList.Any())
                {
                    await mongoCollection.InsertManyAsync(alertList);
                    Console.WriteLine($"✅ Migrated {alertList.Count} alerts");
                }
                else
                {
                    Console.WriteLine("⚪ No alerts found in CSV");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to migrate alerts: {ex.Message}");
            }
        }

        private async Task MigrateRetailersAsync()
        {
            try
            {
                var retailers = await _csvService.GetAllAsync<Retailer>("Retailer_export.csv");
                var mongoCollection = _mongoDbService.GetCollection<Retailer>("retailer");

                var retailerList = retailers.ToList();
                if (retailerList.Any())
                {
                    await mongoCollection.InsertManyAsync(retailerList);
                    Console.WriteLine($"✅ Migrated {retailerList.Count} retailers");
                }
                else
                {
                    Console.WriteLine("⚪ No retailers found in CSV");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to migrate retailers: {ex.Message}");
            }
        }

        private async Task MigrateReviewsAsync()
        {
            try
            {
                var reviews = await _csvService.GetAllAsync<Review>("Review_export.csv");
                var mongoCollection = _mongoDbService.GetCollection<Review>("review");

                var reviewList = reviews.ToList();
                if (reviewList.Any())
                {
                    await mongoCollection.InsertManyAsync(reviewList);
                    Console.WriteLine($"✅ Migrated {reviewList.Count} reviews");
                }
                else
                {
                    Console.WriteLine("⚪ No reviews found in CSV");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to migrate reviews: {ex.Message}");
            }
        }

        private async Task MigratePriceHistoryAsync()
        {
            try
            {
                var priceHistory = await _csvService.GetAllAsync<PriceHistory>("PriceHistory_export.csv");
                var mongoCollection = _mongoDbService.GetCollection<PriceHistory>("pricehistory");

                var priceHistoryList = priceHistory.ToList();
                if (priceHistoryList.Any())
                {
                    await mongoCollection.InsertManyAsync(priceHistoryList);
                    Console.WriteLine($"✅ Migrated {priceHistoryList.Count} price history records");
                }
                else
                {
                    Console.WriteLine("⚪ No price history found in CSV");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to migrate price history: {ex.Message}");
            }
        }

        private async Task MigratePriceCacheAsync()
        {
            try
            {
                var priceCache = await _csvService.GetAllAsync<PriceCache>("PriceCache_export.csv");
                var mongoCollection = _mongoDbService.GetCollection<PriceCache>("pricecache");

                var priceCacheList = priceCache.ToList();
                if (priceCacheList.Any())
                {
                    await mongoCollection.InsertManyAsync(priceCacheList);
                    Console.WriteLine($"✅ Migrated {priceCacheList.Count} price cache records");
                }
                else
                {
                    Console.WriteLine("⚪ No price cache found in CSV");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to migrate price cache: {ex.Message}");
            }
        }
    }
}