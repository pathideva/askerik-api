using AskErik.Api.Common;
using AskErik.Api.Models;
using AskErik.Api.Repositories;

namespace AskErik.Api.Lessons;

public class CreateLesson : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/lessons", Handle)
            .WithSummary("Creates a new lesson");
    }

    private static async Task<IResult> Handle(ILessonRepository repo, CreateLessonRequest request)
    {
        var lesson = new Lesson
        {
            Id = string.IsNullOrWhiteSpace(request.Id) ? Guid.NewGuid().ToString() : request.Id,
            Slug = request.Slug,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Duration = request.Duration,
            Difficulty = request.Difficulty,
            Image = request.Image,
            VideoUrl = request.VideoUrl,
            Rating = request.Rating,
            ReviewCount = request.ReviewCount,
            WorkshopId = request.WorkshopId,
            ExpertId = request.ExpertId,
            // Collections (Tools, Materials, Steps, HelperIds, CommentIds) are intentionally left empty/default
            CreatedAt = request.CreatedAt == default ? DateTime.UtcNow : request.CreatedAt,
            UpdatedAt = request.UpdatedAt == default ? DateTime.UtcNow : request.UpdatedAt,
            PublishedAt = request.PublishedAt,
            IsPublished = request.IsPublished
        };

        await repo.CreateAsync(lesson);

        return Results.Created($"/lessons/{lesson.Id}", lesson);
    }

    public record CreateLessonRequest(
        string Id,
        string Slug,
        string Title,
        string Description,
        string Category,
        string Duration,
        Difficulty Difficulty,
        string Image,
        string VideoUrl,
        double Rating,
        int ReviewCount,
        string WorkshopId,
        string ExpertId,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        DateTime? PublishedAt,
        bool IsPublished
    );
}
