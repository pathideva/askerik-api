using AskErik.LessonApi.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// In development use Redis-backed repository instead of Cosmos for faster local iteration
#if DEBUG
// Reuse an existing IConnectionMultiplexer registration from the AppHost if present.
builder.AddRedisClient("cache");
builder.Services.AddScoped<AskErik.LessonApi.Repositories.ILessonRepository, AskErik.LessonApi.Repositories.RedisLessonRepository>();
#endif

// Configure Cosmos DB settings and register Cosmos repository only when NOT running in Development
if (!builder.Environment.IsDevelopment())
{
    // Configure Cosmos DB settings
    builder.Services.Configure<CosmosSettings>(builder.Configuration.GetSection("Cosmos"));

    // Register CosmosClient and repository when configuration is present
    var cosmosSettings = new CosmosSettings();
    builder.Configuration.GetSection("Cosmos").Bind(cosmosSettings);
    if (!string.IsNullOrEmpty(cosmosSettings.AccountEndpoint) && !string.IsNullOrEmpty(cosmosSettings.AccountKey))
    {
        var cosmosClient = new CosmosClient(cosmosSettings.AccountEndpoint, cosmosSettings.AccountKey);
        builder.Services.AddSingleton(cosmosClient);
        builder.Services.AddScoped<AskErik.LessonApi.Repositories.ILessonRepository, AskErik.LessonApi.Repositories.CosmosLessonRepository>(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            return new AskErik.LessonApi.Repositories.CosmosLessonRepository(client, cosmosSettings);
        });
    }
}


var app = builder.Build();

// Seed development data when running locally
if (app.Environment.IsDevelopment())
{
    try
    {
        AskErik.LessonApi.Dev.DevDataSeeder.SeedSampleLessonsAsync(app.Services).GetAwaiter().GetResult();
    }
    catch
    {
        // swallow seeding errors so dev startup isn't blocked
    }
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // In non-development expose the swagger JSON at /swagger/v1/swagger.json
    app.UseSwagger();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
