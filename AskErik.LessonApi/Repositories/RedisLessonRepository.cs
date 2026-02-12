using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AskErik.LessonApi.Models;
using StackExchange.Redis;

namespace AskErik.LessonApi.Repositories;

public class RedisLessonRepository : ILessonRepository
{
    private readonly IDatabase _db;
    private const string Prefix = "lesson:";

    public RedisLessonRepository(IConnectionMultiplexer multiplexer)
    {
        _db = multiplexer.GetDatabase();
    }

    public Task CreateAsync(Lesson lesson)
    {
        var json = JsonSerializer.Serialize(lesson);
        return _db.StringSetAsync(Prefix + lesson.Id, json);
    }

    public async Task DeleteAsync(string id)
    {
        await _db.KeyDeleteAsync(Prefix + id);
    }

    public async Task<Lesson?> GetByIdAsync(string id)
    {
        var val = await _db.StringGetAsync(Prefix + id);
        if (val.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<Lesson>((string)val)!;
    }

    public async Task<Lesson?> GetBySlugAsync(string slug)
    {
        // naive scan -- acceptable for dev only
        var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints()[0]);
        foreach (var key in server.Keys(pattern: Prefix + "*"))
        {
            var val = await _db.StringGetAsync(key);
            if (val.IsNullOrEmpty) continue;
            var lesson = JsonSerializer.Deserialize<Lesson>((string)val);
            if (lesson?.Slug == slug) return lesson;
        }

        return null;
    }

    public async Task<IEnumerable<Lesson>> ListByWorkshopAsync(string workshopId)
    {
        var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints()[0]);
        var results = new List<Lesson>();
        foreach (var key in server.Keys(pattern: Prefix + "*"))
        {
            var val = await _db.StringGetAsync(key);
            if (val.IsNullOrEmpty) continue;
            var lesson = JsonSerializer.Deserialize<Lesson>((string)val);
            if (lesson?.WorkshopId == workshopId) results.Add(lesson);
        }

        return results;
    }

    public Task UpdateAsync(Lesson lesson)
    {
        var json = JsonSerializer.Serialize(lesson);
        return _db.StringSetAsync(Prefix + lesson.Id, json);
    }
}
