using Core.Entities;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LessonsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Lesson>>> GetLessons()
    {
        return await _context.Lessons
            .Include(l => l.Discipline)
            .Include(l => l.Group)
            .Include(l => l.Teacher)
            .Include(l => l.Auditorium)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Lesson>> GetLessonById(int id)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Discipline)
            .Include(l => l.Group)
            .Include(l => l.Teacher)
            .Include(l => l.Auditorium)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
        {
            return NotFound();
        }
        
        return lesson;
    }
    
    [HttpPost]
    public async Task<ActionResult<Lesson>> CreateLesson([FromBody] Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Id }, lesson);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLesson(int id, [FromBody] Lesson lesson)
    {
        if (id != lesson.Id)
        {
            return BadRequest();
        }
        
        _context.Entry(lesson).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Lessons.Any(l => l.Id == id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLesson(int id)
    {
        var lesson = await _context.Lessons.FindAsync(id);

        if (lesson == null)
        {
            return NotFound();
        }
        
        _context.Lessons.Remove(lesson);
        
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}