using Core.Entities;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/lessons")]
public class LessonsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LessonsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Lesson>>> GetLessons([FromQuery] int? groupId = null, [FromQuery] int? teacherId = null, [FromQuery] int? dayId = null)
    {
        var query = _context.Lessons
            .Include(l => l.Group)
            .Include(l => l.Discipline)
            .Include(l => l.Teacher)
            .Include(l => l.Auditorium)
            .AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(l => l.GroupId == groupId.Value);
        }

        if (teacherId.HasValue)
        {
            query = query.Where(l => l.TeacherId == teacherId.Value);
        }

        if (dayId.HasValue)
        {
            query = query.Where(l => l.DayId == dayId.Value);
        }
        
        var lessons = await query.ToListAsync();

        if (lessons.Count == 0)
        {
            return NotFound();
        }
        
        return Ok(lessons);
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
        
        return Ok(lesson);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateLesson([FromBody] Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Id }, lesson);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLesson(int id, [FromBody] Lesson updatedLesson)
    {
        var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
        {
            return NotFound();
        }
        
        lesson.Number = updatedLesson.Number;
        lesson.DisciplineId = updatedLesson.DisciplineId;
        lesson.TeacherId = updatedLesson.TeacherId;
        lesson.GroupId = updatedLesson.GroupId;
        lesson.AuditoriumId = updatedLesson.AuditoriumId;
        lesson.DayId = updatedLesson.DayId;
        
        await _context.SaveChangesAsync();

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