using Core.Entities;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/groups")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public GroupsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Group>>> GetGroups([FromQuery] int? courseId)
    {
        var query = _context.Groups
            .Include(g => g.Course)
            .AsQueryable();

        if (courseId.HasValue)
        {
            query = query.Where(g => g.Course.Id == courseId.Value);
        }
        
        var groups = await query.OrderBy(g => g.Number).ToListAsync();
        
        if (groups.Count == 0)
        {
            return NotFound();
        }
        return Ok(groups);
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
    
    [HttpGet("by-number/{number}")]
    public async Task<ActionResult<Group>> GetGroupByNumber(int number)
    {
        var group = await _context.Groups.Include(g => g.Course).FirstOrDefaultAsync(g => g.Number == number);

        if (group == null)
        {
            return NotFound();
        }
        
        return Ok(group);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] Group group)
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
    public async Task<ActionResult<Group>> UpdateGroup(int id, [FromBody] Group updatedGroup)
    {
        var group = await _context.Groups.FindAsync(id);

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