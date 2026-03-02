
using AskErik.Api.Models;
using AskErik.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace AskErik.Api.Repositories;

public class PostgresLessonRepository : ILessonRepository
{
    private readonly PostgresLessonContext _context;

    public PostgresLessonRepository(PostgresLessonContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson is null) return;
        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task<Lesson?> GetByIdAsync(string id)
    {
        return await _context.Lessons.FindAsync(id);
    }

    public async Task<Lesson?> GetBySlugAsync(string slug)
    {
        return await _context.Lessons.FirstOrDefaultAsync(l => l.Slug == slug);
    }

    public async Task<IEnumerable<Lesson>> ListByWorkshopAsync(string workshopId)
    {
        return await _context.Lessons.Where(l => l.WorkshopId == workshopId).ToListAsync();
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();
    }
}
