using AskErik.Api;
using AskErik.Api.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

//add serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostgresLessonContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("tellerikdb")));

builder.Services.AddScoped<AskErik.Api.Repositories.ILessonRepository, AskErik.Api.Repositories.PostgresLessonRepository>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Expose Swagger JSON and UI in development
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//api endopoints
app.MapEndpoints();

app.Run();
