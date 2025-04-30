using Core.Entities;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DepartmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
    {
        return Ok(await _context.Departments.ToListAsync());
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<Department>> GetDepartmentByName(string name)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(x => x.Name == name);

        if (department == null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Department>> GetDepartmentById(int id)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<Department>> CreateDepartment([FromBody] Department department)
    {
        if (await _context.Departments.AnyAsync(d => d.Name == department.Name))
        {
            return Conflict();
        }
        
        _context.Departments.Add(department);

        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, department);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, [FromBody] Department updatedDepartment)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return NotFound();
        }
        
        department.Name = updatedDepartment.Name;

        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return NotFound();
        }

        _context.Departments.Remove(department);

        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}