using Core.Entities;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/auditoriums")]
public class AuditoriumsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuditoriumsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Auditorium>>> GetAuditoriums()
    {
        return Ok(await _context.Auditoriums.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Auditorium>> GetAuditoriumById(int id)
    {
        var auditorium = await _context.Auditoriums.FirstOrDefaultAsync(aud => aud.Id == id);

        if (auditorium == null)
        {
            return NotFound();
        }

        return Ok(auditorium);
    }

    [HttpPost]
    public async Task<ActionResult<Teacher>> CreateAuditorium([FromBody] Auditorium auditorium)
    {
        if (await _context.Auditoriums.AnyAsync(aud => aud.Number == auditorium.Number))
        {
            return Conflict();
        }
        
        _context.Auditoriums.Add(auditorium);
        
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetAuditoriumById), new {id = auditorium.Id}, auditorium);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuditorium(int id, [FromBody] Auditorium updatedAuditorium)
    {
        var auditorium = await _context.Auditoriums.FirstOrDefaultAsync(aud => aud.Id == id);

        if (auditorium == null)
        {
            return NotFound();
        }
        
        auditorium.Number = updatedAuditorium.Number;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuditorium(int id)
    {
        var auditorium = await _context.Auditoriums.FirstOrDefaultAsync(aud => aud.Id == id);

        if (auditorium == null)
        {
            return NotFound();
        }
        
        _context.Auditoriums.Remove(auditorium);
        
        await _context.SaveChangesAsync();

        return NoContent();
    }
}