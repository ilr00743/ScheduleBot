using Core.Entities;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/disciplines")]
public class DisciplinesController : ControllerBase
{
    private readonly AppDbContext _context;

    public DisciplinesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Discipline>>> GetDisciplines()
    {
        return Ok(await _context.Disciplines.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Discipline>> GetDisciplineById(int id)
    {
        var discipline = await _context.Disciplines.FirstOrDefaultAsync(discipline => discipline.Id == id);

        if (discipline == null)
        {
            return NotFound();
        }

        return Ok(discipline);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDiscipline([FromBody] Discipline discipline)
    {
        if (await _context.Disciplines.AnyAsync(d => d.Name == discipline.Name))
        {
            return Conflict();
        }

        _context.Disciplines.Add(discipline);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDisciplineById), new { id = discipline.Id }, discipline);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDiscipline(int id, [FromBody] Discipline updatedDiscipline)
    {
        var discipline = await _context.Disciplines.FirstOrDefaultAsync(d => d.Id == id);

        if (discipline == null)
        {
            return NotFound();
        }
        
        discipline.Name = updatedDiscipline.Name;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDiscipline(int id)
    {
        var discipline = await _context.Disciplines.FirstOrDefaultAsync(d => d.Id == id);

        if (discipline == null)
        {
            return NotFound();
        }
        
        _context.Disciplines.Remove(discipline);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}