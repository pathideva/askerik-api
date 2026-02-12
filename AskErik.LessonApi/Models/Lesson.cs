namespace AskErik.LessonApi.Models;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Lesson
{
    // Core Identification
    // Cosmos DB requires an `id` string property. Keep this as the document id.
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    // Basic Info
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }

    // Media
    public string Image { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;

    // Ratings & Reviews
    public double Rating { get; set; }
    public int ReviewCount { get; set; }

    // Requirements
    public List<string> Tools { get; set; } = new();
    public List<Material> Materials { get; set; } = new();

    // Content
    public List<string> Steps { get; set; } = new();

    // Relationships (IDs for full data)
    public string WorkshopId { get; set; } = string.Empty;
    public string ExpertId { get; set; } = string.Empty;
    public List<string> HelperIds { get; set; } = new();
    public List<string> CommentIds { get; set; } = new();

    // Denormalized data for display
    public LessonWorkshop? Workshop { get; set; }
    public LessonExpert? Expert { get; set; }
    public List<LessonHelper> Helpers { get; set; } = new();

    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }

    // Note: choose a partition key with high cardinality. Example: WorkshopId or Category.
    // You can add a computed property to expose the partition key used when writing to Cosmos.
    [JsonIgnore]
    public string PartitionKey => WorkshopId ?? string.Empty;
}
