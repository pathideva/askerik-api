using AskErik.Api.Common;
using AskErik.Api.Filters;
using AskErik.Api.Lessons;
using Microsoft.AspNetCore.Builder;

namespace AskErik.Api;
public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints =app.MapGroup("api")
           .AddEndpointFilter<RequestLoggingFilter>();

        endpoints.MapLessonEndpoints();
    }

    private static void MapLessonEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/lessons");

        endpoints.MapPublicGroup()
            .MapEndpoint<GetLesson>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }

    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }
}