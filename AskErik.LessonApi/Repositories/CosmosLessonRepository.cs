using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskErik.LessonApi.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace AskErik.LessonApi.Repositories;

public class CosmosLessonRepository : ILessonRepository
{
    private readonly CosmosClient _client;
    private readonly Container _container;

    public CosmosLessonRepository(CosmosClient client, CosmosSettings settings)
    {
        _client = client;
        var database = _client.GetDatabase(settings.DatabaseName);
        _container = database.GetContainer(settings.ContainerName);
    }

    public async Task CreateAsync(Lesson lesson)
    {
        await _container.CreateItemAsync(lesson, new PartitionKey(lesson.PartitionKey));
    }

    public async Task DeleteAsync(string id)
    {
        // need partition key; attempt to read then delete
        var item = await GetByIdAsync(id);
        if (item is null) return;
        await _container.DeleteItemAsync<Lesson>(id, new PartitionKey(item.PartitionKey));
    }

    public async Task<Lesson?> GetByIdAsync(string id)
    {
        try
        {
            var iterator = _container.GetItemLinqQueryable<Lesson>(allowSynchronousQueryExecution: false)
                .Where(l => l.Id == id)
                .ToFeedIterator();

            if (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                return response.FirstOrDefault();
            }

            return null;
        }
        catch (CosmosException)
        {
            return null;
        }
    }

    public async Task<Lesson?> GetBySlugAsync(string slug)
    {
        var iterator = _container.GetItemLinqQueryable<Lesson>(allowSynchronousQueryExecution: false)
            .Where(l => l.Slug == slug)
            .ToFeedIterator();

        if (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            return response.FirstOrDefault();
        }

        return null;
    }

    public async Task<IEnumerable<Lesson>> ListByWorkshopAsync(string workshopId)
    {
        var iterator = _container.GetItemLinqQueryable<Lesson>(allowSynchronousQueryExecution: false)
            .Where(l => l.WorkshopId == workshopId)
            .ToFeedIterator();

        var results = new List<Lesson>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        await _container.UpsertItemAsync(lesson, new PartitionKey(lesson.PartitionKey));
    }
}
