using Core.Entities;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public GroupsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
    {
        return await _context.Groups.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Group>> GetGroupById(int id)
    {
        var group = await _context.Groups.Include(g => g.Course).FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            return NotFound();
        }
        
        return Ok(group);
    }

    [HttpPost]
    public async Task<ActionResult<Group>> PostGroup([FromBody] Group group)
    {
        if (await _context.Groups.AnyAsync(g => g.Id == group.Id))
        {
            return Conflict();
        }
        
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGroupById), new { id = group.Id }, group);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Group>> PutGroup(int id, [FromBody] Group updatedGroup)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            return NotFound();
        }
        
        group.Number = updatedGroup.Number;
        
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Group>> DeleteGroup(int id)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            return NotFound();
        }
        
        _context.Groups.Remove(group);
        
        await _context.SaveChangesAsync();
        return NoContent();
    }
}