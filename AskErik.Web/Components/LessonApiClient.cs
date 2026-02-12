using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Threading;
using AskErik.Web.Models;

namespace AskErik.Web
{
    public class LessonApiClient
    {
        private readonly HttpClient _httpClient;

        public LessonApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Example method - replace or extend as needed
        public async Task<string> GetLessonAsync(string lessonId)
        {
            var response = await _httpClient.GetAsync($"/lessons/{lessonId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        
        public async Task<LessonDto[]> GetLessonsAsync(CancellationToken cancellationToken = default)
        {
            var lessons = await _httpClient.GetFromJsonAsync<LessonDto[]>("/lessons", cancellationToken);
            return lessons ?? System.Array.Empty<LessonDto>();
        }
    }
}