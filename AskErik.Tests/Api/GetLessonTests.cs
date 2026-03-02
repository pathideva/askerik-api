using System.IO;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using AskErik.Api.Lessons;
using AskErik.Api.Repositories;
using AskErik.Api.Models;

namespace AskErik.Tests.Api;

public class GetLessonTests
{
    [Fact]
    public async Task Handle_Returns_Ok_When_Lesson_Found()
    {
        // Arrange
        var repo = new Mock<ILessonRepository>();
        var lesson = new Lesson { Id = "1", Title = "Test" };
        repo.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(lesson);

        var method = typeof(GetLesson).GetMethod("Handle", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var taskObj = method!.Invoke(null, new object[] { repo.Object, "1" })!;
        var task = (Task<IResult>)taskObj;
        var result = await task;

        var ctx = new DefaultHttpContext();
        ctx.Response.Body = new MemoryStream();

        // Act
        await result.ExecuteAsync(ctx);

        // Assert
        Assert.Equal(200, ctx.Response.StatusCode);

        ctx.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(ctx.Response.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();
        Assert.Contains("\"Title\":\"Test\"", body);
    }

    [Fact]
    public async Task Handle_Returns_NotFound_When_Lesson_Missing()
    {
        // Arrange
        var repo = new Mock<ILessonRepository>();
        repo.Setup(r => r.GetByIdAsync("does-not-exist")).ReturnsAsync((Lesson?)null);

        var method = typeof(GetLesson).GetMethod("Handle", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var taskObj = method!.Invoke(null, new object[] { repo.Object, "does-not-exist" })!;
        var task = (Task<IResult>)taskObj;
        var result = await task;

        var ctx = new DefaultHttpContext();
        ctx.Response.Body = new MemoryStream();

        // Act
        await result.ExecuteAsync(ctx);

        // Assert
        Assert.Equal(404, ctx.Response.StatusCode);
    }
}
