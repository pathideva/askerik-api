using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AskErik.LessonApi.Models;
using AskErik.LessonApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AskErik.LessonApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonController : ControllerBase
{
    private readonly ILessonRepository _repository;

    public LessonController(ILessonRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Lesson>> GetById(string id)
    {
        var lesson = await _repository.GetByIdAsync(id);
        if (lesson is null) return NotFound();
        return Ok(lesson);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<Lesson>> GetBySlug(string slug)
    {
        var lesson = await _repository.GetBySlugAsync(slug);
        if (lesson is null) return NotFound();
        return Ok(lesson);
    }

    [HttpGet("workshop/{workshopId}")]
    public async Task<ActionResult<IEnumerable<Lesson>>> ListByWorkshop(string workshopId)
    {
        var items = await _repository.ListByWorkshopAsync(workshopId);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<Lesson>> Create([FromBody] Lesson lesson)
    {
        if (lesson is null) return BadRequest();

        if (string.IsNullOrWhiteSpace(lesson.Id)) lesson.Id = Guid.NewGuid().ToString();
        lesson.CreatedAt = DateTime.UtcNow;
        lesson.UpdatedAt = lesson.CreatedAt;

        await _repository.CreateAsync(lesson);

        return CreatedAtAction(nameof(GetById), new { id = lesson.Id }, lesson);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Lesson lesson)
    {
        if (lesson is null || id != lesson.Id) return BadRequest();

        lesson.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(lesson);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
