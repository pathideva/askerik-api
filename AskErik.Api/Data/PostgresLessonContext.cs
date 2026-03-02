using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AskErik.Api.Models;

namespace AskErik.Api.Data;

public class PostgresLessonContext : DbContext
{
    public PostgresLessonContext(DbContextOptions<PostgresLessonContext> options) : base(options)
    {
    }

    // Use plural DbSet name so repository code (_context.Lessons) matches the context.
    public DbSet<Lesson> Lessons { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map to a lowercase singular table name to match Postgres conventions.
        modelBuilder.Entity<Lesson>().ToTable("lesson");

        // Convert the List<Material> into a jsonb column so EF doesn't try to treat Material as an entity.
        var materialsConverter = new ValueConverter<List<Material>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<Material>>(v, (JsonSerializerOptions?)null) ?? new List<Material>());

        // Provide a ValueComparer so EF can detect changes, compute hashes and snapshot the list.
        var materialsComparer = new ValueComparer<List<Material>>(
            (l1, l2) =>
                JsonSerializer.Serialize(l1, (JsonSerializerOptions?)null)
                == JsonSerializer.Serialize(l2, (JsonSerializerOptions?)null),
            l => l == null ? 0 : JsonSerializer.Serialize(l, (JsonSerializerOptions?)null).GetHashCode(),
            l => l == null
                ? new List<Material>()
                : JsonSerializer.Deserialize<List<Material>>(JsonSerializer.Serialize(l, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null) ?? new List<Material>());

        var materialsProperty = modelBuilder.Entity<Lesson>().Property(l => l.Materials);
        materialsProperty.HasConversion(materialsConverter)
                         .HasColumnType("jsonb");
        materialsProperty.Metadata.SetValueComparer(materialsComparer);

        base.OnModelCreating(modelBuilder);
    }
}

