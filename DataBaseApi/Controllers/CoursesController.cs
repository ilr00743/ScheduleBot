using Core.DTO;
using Core.Entities;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
    {
        var courses = await _context.Courses
            .Include(c => c.Groups)
            .ToListAsync();
        
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Course>> GetCourseById(int id)
    {
        var course = await _context.Courses
            .FindAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        return Ok(course);
    }
    
    [HttpGet("by-number/{number}")]
    public async Task<ActionResult<Course>> GetCourseByNumber(int number)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Number == number);

        if (course == null)
        {
            return NotFound();
        }

        return Ok(course);
    }

    [HttpPost]
    public async Task<ActionResult<Course>> CreateCourse([FromBody] Course course)
    {
        if (await _context.Courses.AnyAsync(c => c.Number == course.Number))
        {
            return Conflict();
        }
        
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseRequest request)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        if (request.Number.HasValue)
        {
            course.Number = request.Number.Value;
        }

        if (request.Groups != null)
        {
            course.Groups = request.Groups;
        }
        
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
        {
            return NotFound();
        }
        
        _context.Courses.Remove(course);
        
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}