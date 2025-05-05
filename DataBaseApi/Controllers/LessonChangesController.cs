using Core.Entities;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/changes")]
public class LessonChangesController : ControllerBase
{
     private readonly AppDbContext _context;

    public LessonChangesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonChange>>> GetLessonsChanges([FromQuery] int? groupId, [FromQuery] int? teacherId, [FromQuery] DateOnly? date)
    {
        var query = _context.LessonsChanges
            .Include(l => l.Group)
            .Include(l => l.Discipline)
            .Include(l => l.Teacher)
            .Include(l => l.Auditorium)
            .AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(lc => lc.GroupId == groupId.Value);
        }

        if (teacherId.HasValue)
        {
            query = query.Where(lc => lc.TeacherId == teacherId.Value);
        }

        if (date.HasValue)
        {
            query = query.Where(lc => lc.Date == date.Value);
        }
        
        var lessonChanges = await query.ToListAsync();

        if (lessonChanges.Count == 0)
        {
            return NotFound();
        }
        
        return Ok(lessonChanges);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonChange>> GetLessonChangesById(int id)
    {
        var lessonChanges = await _context.LessonsChanges
            .Include(l => l.Discipline)
            .Include(l => l.Group)
            .Include(l => l.Teacher)
            .Include(l => l.Auditorium)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lessonChanges == null)
        {
            return NotFound();
        }
        
        return Ok(lessonChanges);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateLessonChanges([FromBody] LessonChange lessonChange)
    {
        _context.LessonsChanges.Add(lessonChange);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetLessonChangesById), new { id = lessonChange.Id }, lessonChange);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLessonChanges(int id, [FromBody] LessonChange updatedLesson)
    {
        var change = await _context.LessonsChanges.FirstOrDefaultAsync(l => l.Id == id);

        if (change == null)
        {
            return NotFound();
        }
        
        change.Number = updatedLesson.Number;
        change.DisciplineId = updatedLesson.DisciplineId;
        change.TeacherId = updatedLesson.TeacherId;
        change.GroupId = updatedLesson.GroupId;
        change.AuditoriumId = updatedLesson.AuditoriumId;
        
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLessonChanges(int id)
    {
        var change = await _context.LessonsChanges.FindAsync(id);

        if (change == null)
        {
            return NotFound();
        }
        
        _context.LessonsChanges.Remove(change);
        
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}