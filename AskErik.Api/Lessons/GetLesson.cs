using AskErik.Api.Common;
using AskErik.Api.Repositories;

namespace AskErik.Api.Lessons;

public class GetLesson : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/lessons/{id}", Handle)
            .WithSummary("Gets a lesson by its ID");
    }

    private static async Task<IResult> Handle(ILessonRepository repo, string id)
    {
        var lesson = await repo.GetByIdAsync(id);
        return lesson is null ? Results.NotFound() : Results.Ok(lesson);
    }
}
