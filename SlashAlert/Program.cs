using Microsoft.Azure.Cosmos;
using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;
using SlashAlert.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using UserModel = SlashAlert.Models.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "SlashAlert API", 
        Version = "v1",
        Description = "OAuth User Management and Export API for SlashAlert with Google JWT Authentication"
    });
    
    // Add Google JWT Bearer Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"Google JWT Authorization header using the Bearer scheme. 
                       Enter your Google-issued JWT token in the text input below.
                       Example: 'Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
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

// Configure Database Settings
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(DatabaseSettings.SectionName));

// Configure Google OAuth Settings
builder.Services.Configure<GoogleOAuthSettings>(
    builder.Configuration.GetSection(GoogleOAuthSettings.SectionName));

// Register Google Token Validation Service
builder.Services.AddScoped<IGoogleTokenValidationService, GoogleTokenValidationService>();

// Configure Authentication and Authorization
var googleOAuthSettings = builder.Configuration.GetSection(GoogleOAuthSettings.SectionName).Get<GoogleOAuthSettings>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com";
        options.Audience = googleOAuthSettings?.ClientId;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = googleOAuthSettings?.ValidIssuers ?? new List<string> { "https://accounts.google.com" },
            ValidateAudience = !googleOAuthSettings?.DisableAudienceValidation ?? true,
            ValidAudiences = googleOAuthSettings?.ValidAudiences ?? new List<string>(),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
        
        // Add events for debugging
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    
    // Add custom policies if needed
    options.AddPolicy("RequireGoogleAuth", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("iss", "https://accounts.google.com"));
});

// Configure Cosmos DB (legacy configuration for backward compatibility)
builder.Services.Configure<CosmosDbSettings>(
    builder.Configuration.GetSection("CosmosDb"));

// Register CSV Service
builder.Services.AddScoped<ICsvService, CsvService>();

// Configure Cosmos DB Client (only if CosmosDB provider is selected)
var databaseSettings = builder.Configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
if (databaseSettings?.Provider?.ToUpper() == "COSMOSDB")
{
    builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
    {
        var cosmosDbSettings = databaseSettings.CosmosDb ?? 
                              builder.Configuration.GetSection("CosmosDb").Get<CosmosDbSettings>();
        
        // Try to get connection string from environment variables (for production)
        var endpoint = Environment.GetEnvironmentVariable("COSMOS_DB_ENDPOINT") ?? cosmosDbSettings?.EndpointUri;
        var key = Environment.GetEnvironmentVariable("COSMOS_DB_PRIMARY_KEY") ?? cosmosDbSettings?.PrimaryKey;
        
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("Cosmos DB endpoint and key must be configured either in appsettings.json or as environment variables.");
        }
        
        return new CosmosClient(endpoint, key);
    });

    builder.Services.AddScoped<Container>(serviceProvider =>
    {
        var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
        var cosmosDbSettings = databaseSettings.CosmosDb ?? 
                              builder.Configuration.GetSection("CosmosDb").Get<CosmosDbSettings>();
        return cosmosClient.GetContainer(cosmosDbSettings?.DatabaseName ?? "SlashAlert", 
                                       cosmosDbSettings?.ContainerName ?? "Items");
    });

    // Legacy service for backward compatibility (only if CosmosDB is used)
    builder.Services.AddScoped<ICosmosDbService, CosmosDbService>();
}

// Register Repository Factory
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

// Register individual repositories using factory
builder.Services.AddScoped<IAlertRepository>(provider => 
    provider.GetRequiredService<IRepositoryFactory>().CreateAlertRepository());
builder.Services.AddScoped<IProductRepository>(provider => 
    provider.GetRequiredService<IRepositoryFactory>().CreateProductRepository());
builder.Services.AddScoped<IRetailerRepository>(provider => 
    provider.GetRequiredService<IRepositoryFactory>().CreateRetailerRepository());
builder.Services.AddScoped<IReviewRepository>(provider => 
    provider.GetRequiredService<IRepositoryFactory>().CreateReviewRepository());
builder.Services.AddScoped<IPriceHistoryRepository>(provider => 
    provider.GetRequiredService<IRepositoryFactory>().CreatePriceHistoryRepository());
builder.Services.AddScoped<IPriceCacheRepository>(provider => 
    provider.GetRequiredService<IRepositoryFactory>().CreatePriceCacheRepository());
builder.Services.AddScoped<IUserRepository>(provider => 
    provider.GetRequiredService<IRepositoryFactory>().CreateUserRepository());

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable CORS
app.UseCors("AllowAll");

// Add Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Add custom JWT middleware for additional token validation
app.UseMiddleware<JwtAuthenticationMiddleware>();

// Enable Swagger in all environments for API documentation
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SlashAlert API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "SlashAlert API Documentation - Google JWT Auth";
    c.DefaultModelsExpandDepth(-1); // Hide models section
    c.DisplayRequestDuration(); // Show request duration
    c.EnableDeepLinking(); // Enable deep linking
    c.EnableValidator(); // Enable validator
    
    // Configure OAuth2 (for reference - users will use Bearer token)
    c.OAuthClientId("208621379401-mundn85bk9cbgmoarea2esilgi8e8vqhj.apps.googleusercontent.com");
    c.OAuthAppName("SlashAlert API");
    c.OAuthScopeSeparator(" ");
    c.OAuthUsePkce();
});

// Redirect root to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
    .WithName("RedirectToSwagger")
    .WithSummary("Redirect to Swagger documentation")
    .ExcludeFromDescription();

// OAuth User API endpoints (now using repository pattern)
app.MapGet("/api/users", async (IUserRepository userRepository) =>
{
    var users = await userRepository.GetAllAsync();
    return Results.Ok(users);
})
.WithName("GetUsers")
.WithSummary("Get all OAuth users")
.WithDescription("Retrieves all OAuth users from the configured database")
.RequireAuthorization() // Require authentication
.WithOpenApi();

app.MapGet("/api/users/{id}/{partitionKey}", async (string id, string partitionKey, IUserRepository userRepository) =>
{
    var user = await userRepository.GetByPartitionKeyAsync(id, partitionKey);
    return user != null ? Results.Ok(user) : Results.NotFound($"User with ID {id} not found");
})
.WithName("GetUser")
.WithSummary("Get OAuth user by ID and partition key")
.WithDescription("Retrieves a specific OAuth user by their ID and partition key")
.RequireAuthorization() // Require authentication
.WithOpenApi();

app.MapGet("/api/users/sub/{sub}", async (string sub, IUserRepository userRepository) =>
{
    var user = await userRepository.GetBySubAsync(sub);
    return user != null ? Results.Ok(user) : Results.NotFound($"User with subject {sub} not found");
})
.WithName("GetUserBySub")
.WithSummary("Get OAuth user by subject")
.WithDescription("Retrieves a user by their OAuth subject identifier")
.RequireAuthorization() // Require authentication
.WithOpenApi();

app.MapGet("/api/users/email/{email}", async (string email, IUserRepository userRepository) =>
{
    var user = await userRepository.GetByEmailAsync(email);
    return user != null ? Results.Ok(user) : Results.NotFound($"User with email {email} not found");
})
.WithName("GetUserByEmail")
.WithSummary("Get OAuth user by email")
.WithDescription("Retrieves a user by their email address")
.RequireAuthorization() // Require authentication
.WithOpenApi();

app.MapGet("/api/users/provider/{provider}", async (string provider, IUserRepository userRepository) =>
{
    var users = await userRepository.GetByProviderAsync(provider);
    return Results.Ok(users);
})
.WithName("GetUsersByProvider")
.WithSummary("Get OAuth users by provider")
.WithDescription("Retrieves all users from a specific OAuth provider (e.g., 'google', 'facebook', 'twitter')")
.RequireAuthorization() // Require authentication
.WithOpenApi();

app.MapGet("/api/users/active", async (IUserRepository userRepository) =>
{
    var users = await userRepository.GetActiveUsersAsync();
    return Results.Ok(users);
})
.WithName("GetActiveUsers")
.WithSummary("Get active OAuth users")
.WithDescription("Retrieves all active OAuth users")
.RequireAuthorization() // Require authentication
.WithOpenApi();

app.MapGet("/api/users/recent-logins", async (int days, IUserRepository userRepository) =>
{
    var users = await userRepository.GetRecentLoginsAsync(days);
    return Results.Ok(users);
})
.WithName("GetRecentLogins")
.WithSummary("Get users with recent logins")
.WithDescription("Retrieves users who have logged in within the specified number of days (default: 30)")
.RequireAuthorization() // Require authentication
.WithOpenApi();

// Map controllers
app.MapControllers();

app.Run();
