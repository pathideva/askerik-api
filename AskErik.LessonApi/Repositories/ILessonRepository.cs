using System.Collections.Generic;
using System.Threading.Tasks;
using AskErik.LessonApi.Models;

namespace AskErik.LessonApi.Repositories;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(string id);
    Task<Lesson?> GetBySlugAsync(string slug);
    Task<IEnumerable<Lesson>> ListByWorkshopAsync(string workshopId);
    Task CreateAsync(Lesson lesson);
    Task UpdateAsync(Lesson lesson);
    Task DeleteAsync(string id);
}
