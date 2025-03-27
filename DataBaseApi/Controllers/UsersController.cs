using Core.Entities;
using DataBaseApi.DTO;
using DataBaseApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }
        
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(user);
    }

    [HttpGet("telegram/{telegramId}")]
    public async Task<ActionResult<User>> GetUserByTelegramId(string telegramId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody]User user)
    {
        if (await _context.Users.AnyAsync(u => u.TelegramId == user.TelegramId))
        {
            return Conflict("User already exists");
        }
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserByTelegramId), new { telegramId = user.TelegramId }, user);
    }

    [HttpPut("status")]
    public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.TelegramId == request.TelegramId);

        if (user == null)
        {
            return NotFound();
        }

        user.Status = request.UserStatus;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("group")]
    public async Task<IActionResult> UpdateUserGroup([FromBody] UpdateUserGroupRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.TelegramId == request.TelegramId);

        if (user == null)
        {
            return NotFound();
        }

        var group = await _context.Groups.FindAsync(request.GroupId);

        if (group == null)
        {
            return BadRequest("Group not found");
        }
        
        user.GroupId = group.Id;

        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPut("teacher")]
    public async Task<IActionResult> UpdateUserTeacher([FromBody] UpdateUserTeacherRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == request.TelegramId);

        if (user == null)
        {
            return NotFound();
        }

        var teacher = await _context.Teachers.FindAsync(request.TeacherId);

        if (teacher == null)
        {
            return BadRequest("Teacher not found");
        }
        
        user.TeacherId = teacher.Id;

        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}