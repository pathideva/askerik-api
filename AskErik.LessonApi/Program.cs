using AskErik.LessonApi.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostgresLessonContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("tellerikdb")));

builder.Services.AddScoped<AskErik.LessonApi.Repositories.ILessonRepository, AskErik.LessonApi.Repositories.PostgresLessonRepository>();

var app = builder.Build();

// Map minimal API endpoints for lessons
app.MapEndpoints();

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
