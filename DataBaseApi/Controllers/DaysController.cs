using Core.Entities;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/days")]
public class DaysController : ControllerBase
{
    private readonly AppDbContext _context;

    public DaysController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeekDay>>> GetDays()
    {
        var days = await _context.Days.OrderBy(d => d.Id).ToListAsync();

        if (days.Count == 0)
        {
            return NotFound();
        }
        
        return Ok(days);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WeekDay>> GetDayById(int dayId)
    {
        var day = await _context.Days.FindAsync(dayId);

        if (day == null)
        {
            return NotFound();
        }
        
        return Ok(day);
    }

    [HttpGet("by-name")]
    public async Task<ActionResult<WeekDay>> GetDayByName([FromQuery] string dayName)
    {
        var day = await _context.Days.FirstOrDefaultAsync(d => d.Name == dayName);

        if (day == null)
        {
            return NotFound();
        }
        
        return Ok(day);
    }
}