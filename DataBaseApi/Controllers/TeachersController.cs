using Core.Entities;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/teachers")]
public class TeachersController : ControllerBase
{
    private readonly AppDbContext _context;

    public TeachersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Teacher>>> GetTeachers()
    {
        var teachers = await _context.Teachers.ToListAsync();
        return Ok(teachers);
    }

    [HttpGet("by-name")]
    public async Task<ActionResult<Teacher>> GetTeacherByFullName([FromQuery] string fullName)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.FullName == fullName);

        if (teacher == null)
        {
            return NotFound();
        }
        
        return Ok(teacher);
    }

    [HttpGet("by-department/{departmentId}")]
    public async Task<ActionResult<IEnumerable<Teacher>>> GetTeachersByDepartment(int departmentId)
    {
        var teachers = await _context.Teachers
            .Where(t => t.DepartmentId == departmentId).ToListAsync();

        if (teachers.Count == 0)
        {
            return NotFound();
        }
        
        return Ok(teachers);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Teacher>> GetTeacherById(int id)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id);

        if (teacher == null)
        {
            return NotFound();
        }
        
        return Ok(teacher);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeacher([FromBody]Teacher teacher)
    {
        if (await _context.Teachers.AnyAsync(t => t.Id == teacher.Id))
        {
            return Conflict("Teacher already exists");
        }
        _context.Teachers.Add(teacher);
        
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTeacherById), new { id = teacher.Id }, teacher);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeacher(int id, [FromBody] Teacher updatedTeacher)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id);

        if (teacher == null)
        {
            return NotFound();
        }
        
        teacher.FullName = updatedTeacher.FullName;
        teacher.DepartmentId = updatedTeacher.DepartmentId;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id);

        if (teacher == null)
        {
            return NotFound();
        }
        
        _context.Teachers.Remove(teacher);
        
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}