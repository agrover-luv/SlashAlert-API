using Microsoft.Azure.Cosmos;
using SlashAlert.Models;
using SlashAlert.Services;
using UserModel = SlashAlert.Models.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "SlashAlert API", 
        Version = "v1",
        Description = "OAuth User Management API for SlashAlert"
    });
});

// Add CORS for development and production
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Configure Cosmos DB
builder.Services.Configure<CosmosDbSettings>(
    builder.Configuration.GetSection("CosmosDb"));

builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
{
    var cosmosDbSettings = builder.Configuration.GetSection("CosmosDb").Get<CosmosDbSettings>();
    
    // Try to get connection string from environment variables (for production)
    var endpoint = Environment.GetEnvironmentVariable("COSMOS_DB_ENDPOINT") ?? cosmosDbSettings?.EndpointUri;
    var key = Environment.GetEnvironmentVariable("COSMOS_DB_PRIMARY_KEY") ?? cosmosDbSettings?.PrimaryKey;
    
    if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
    {
        throw new InvalidOperationException("Cosmos DB endpoint and key must be configured either in appsettings.json or as environment variables.");
    }
    
    return new CosmosClient(endpoint, key);
});

builder.Services.AddScoped<ICosmosDbService, CosmosDbService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable CORS
app.UseCors("AllowAll");

// Enable Swagger in all environments for API documentation
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SlashAlert API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "SlashAlert API Documentation";
});

// Redirect root to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
    .WithName("RedirectToSwagger")
    .WithSummary("Redirect to Swagger documentation")
    .ExcludeFromDescription();

// OAuth User API endpoints
app.MapGet("/api/users", async (ICosmosDbService cosmosDbService) =>
{
    var users = await cosmosDbService.GetAllUsersAsync();
    return Results.Ok(users);
})
.WithName("GetUsers")
.WithSummary("Get all OAuth users")
.WithDescription("Retrieves all OAuth users from the database")
.WithOpenApi();

app.MapGet("/api/users/{id}/{partitionKey}", async (string id, string partitionKey, ICosmosDbService cosmosDbService) =>
{
    var user = await cosmosDbService.GetUserByIdAsync(id, partitionKey);
    return user != null ? Results.Ok(user) : Results.NotFound($"User with ID {id} not found");
})
.WithName("GetUser")
.WithSummary("Get OAuth user by ID and partition key")
.WithDescription("Retrieves a specific OAuth user by their ID and partition key")
.WithOpenApi();

app.MapGet("/api/users/sub/{sub}", async (string sub, ICosmosDbService cosmosDbService) =>
{
    var user = await cosmosDbService.GetUserBySubAsync(sub);
    return user != null ? Results.Ok(user) : Results.NotFound($"User with subject {sub} not found");
})
.WithName("GetUserBySub")
.WithSummary("Get OAuth user by subject")
.WithDescription("Retrieves a user by their OAuth subject identifier")
.WithOpenApi();

app.MapGet("/api/users/email/{email}", async (string email, ICosmosDbService cosmosDbService) =>
{
    var user = await cosmosDbService.GetUserByEmailAsync(email);
    return user != null ? Results.Ok(user) : Results.NotFound($"User with email {email} not found");
})
.WithName("GetUserByEmail")
.WithSummary("Get OAuth user by email")
.WithDescription("Retrieves a user by their email address")
.WithOpenApi();

app.MapGet("/api/users/provider/{provider}", async (string provider, ICosmosDbService cosmosDbService) =>
{
    var users = await cosmosDbService.GetUsersByProviderAsync(provider);
    return Results.Ok(users);
})
.WithName("GetUsersByProvider")
.WithSummary("Get OAuth users by provider")
.WithDescription("Retrieves all users from a specific OAuth provider (e.g., 'google', 'facebook', 'twitter')")
.WithOpenApi();

app.MapGet("/api/users/active", async (ICosmosDbService cosmosDbService) =>
{
    var users = await cosmosDbService.GetActiveUsersAsync();
    return Results.Ok(users);
})
.WithName("GetActiveUsers")
.WithSummary("Get active OAuth users")
.WithDescription("Retrieves all active OAuth users")
.WithOpenApi();

app.MapGet("/api/users/recent-logins", async (int days, ICosmosDbService cosmosDbService) =>
{
    var users = await cosmosDbService.GetRecentLoginsAsync(days);
    return Results.Ok(users);
})
.WithName("GetRecentLogins")
.WithSummary("Get users with recent logins")
.WithDescription("Retrieves users who have logged in within the specified number of days (default: 30)")
.WithOpenApi();

app.Run();
