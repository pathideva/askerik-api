using Microsoft.AspNetCore.Builder;
using AskErik.LessonApi.Repositories;
using AskErik.LessonApi.Models;

namespace AskErik.LessonApi
{
    public static class Endpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapGet("/api/lesson/{id}", async (ILessonRepository repo, string id) =>
            {
                var lesson = await repo.GetByIdAsync(id);
                return lesson is null ? Results.NotFound() : Results.Ok(lesson);
            });

            app.MapGet("/api/lesson/slug/{slug}", async (ILessonRepository repo, string slug) =>
            {
                var lesson = await repo.GetBySlugAsync(slug);
                return lesson is null ? Results.NotFound() : Results.Ok(lesson);
            });

            app.MapGet("/api/lesson/workshop/{workshopId}", async (ILessonRepository repo, string workshopId) =>
            {
                var items = await repo.ListByWorkshopAsync(workshopId);
                return Results.Ok(items);
            });

            app.MapPost("/api/lesson", async (ILessonRepository repo, Lesson lesson) =>
            {
                if (lesson is null) return Results.BadRequest();
                if (string.IsNullOrWhiteSpace(lesson.Id)) lesson.Id = Guid.NewGuid().ToString();
                lesson.CreatedAt = DateTime.UtcNow;
                lesson.UpdatedAt = lesson.CreatedAt;

                await repo.CreateAsync(lesson);
                return Results.Created($"/api/lesson/{lesson.Id}", lesson);
            });

            app.MapPut("/api/lesson/{id}", async (ILessonRepository repo, string id, Lesson lesson) =>
            {
                if (lesson is null || id != lesson.Id) return Results.BadRequest();
                lesson.UpdatedAt = DateTime.UtcNow;
                await repo.UpdateAsync(lesson);
                return Results.NoContent();
            });

            app.MapDelete("/api/lesson/{id}", async (ILessonRepository repo, string id) =>
            {
                await repo.DeleteAsync(id);
                return Results.NoContent();
            });
        }
    }
}
