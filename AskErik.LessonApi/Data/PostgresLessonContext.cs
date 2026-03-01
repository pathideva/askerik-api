using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using AskErik.LessonApi.Models;

namespace AskErik.LessonApi.Data;

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

        modelBuilder.Entity<Lesson>()
            .Property(l => l.Materials)
            .HasConversion(materialsConverter)
            .HasColumnType("jsonb");

        base.OnModelCreating(modelBuilder);
    }
}

