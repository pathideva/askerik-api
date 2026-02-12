using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AskErik.LessonApi.Models;
using AskErik.LessonApi.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AskErik.LessonApi.Dev;

public static class DevDataSeeder
{
    public static async Task SeedSampleLessonsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var repo = scope.ServiceProvider.GetService<ILessonRepository>();
        if (repo is null) return;

        // naive check: attempt to get any lesson; if exists, skip seeding
        var any = await repo.GetByIdAsync("sample-1");
        if (any is not null) return;

        var samples = new List<Lesson>
        {
            new Lesson
            {
                Id = "sample-1",
                Slug = "how-to-make-a-chair",
                Title = "How to make a chair",
                Description = "Step-by-step guide to build a wooden chair.",
                Category = "Woodworking",
                Duration = "2h",
                Difficulty = Difficulty.Intermediate,
                Image = "",
                VideoUrl = "",
                Rating = 4.5,
                ReviewCount = 12,
                Tools = new List<string>{ "Saw", "Hammer", "Chisel" },
                Materials = new List<Material>{ new Material{ Name = "Wood", Quantity = "5 planks" } },
                Steps = new List<string>{ "Cut wood", "Assemble frame", "Finish" },
                WorkshopId = "w1",
                ExpertId = "e1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsPublished = true,
            },
            new Lesson
            {
                Id = "sample-2",
                Slug = "intro-to-pottery",
                Title = "Intro to pottery",
                Description = "Beginner pottery techniques.",
                Category = "Crafts",
                Duration = "1h",
                Difficulty = Difficulty.Beginner,
                Image = "",
                VideoUrl = "",
                Rating = 4.7,
                ReviewCount = 8,
                Tools = new List<string>{ "Wheel", "Clay" },
                Materials = new List<Material>{ new Material{ Name = "Clay", Quantity = "2 kg" } },
                Steps = new List<string>{ "Prepare clay", "Throw on wheel", "Dry" },
                WorkshopId = "w2",
                ExpertId = "e2",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsPublished = true,
            }
        };

        foreach (var s in samples)
        {
            try
            {
                await repo.CreateAsync(s);
            }
            catch
            {
                // ignore write failures in dev
            }
        }
    }
}
