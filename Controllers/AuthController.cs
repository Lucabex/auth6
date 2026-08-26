using Microsoft.AspNetCore.Mvc;
using auth6.Data;
using auth6.Models;

using Microsoft.EntityFrameworkCore;
using auth6.DTO;


[ApiController]
[Route("auth")]

public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    public AuthController(AppDbContext context)
    {
        _context = context;
    }
    [HttpPost("reg")]
    public async Task<IActionResult> Reg(RegDto dto)
    {
        if(await _context.User.AnyAsync(u=> u.Name.ToLower() == dto.Name.ToLower()))
        {
            return BadRequest("Username already in use");
        }
        var user = new User
        {
            Name = dto.Name,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        return Ok("user registered");
    }

    [HttpPost("log")]
    public async Task<IActionResult> Log(LogDto dto)
    {
        var user = await _context.User.FirstOrDefaultAsync(u=> u.Name.ToLower() == dto.Name.ToLower());
        if(user == null || !BCrypt.Net.BCrypt.Verify(dto.Password , user.HashedPassword))
        {
            return BadRequest("Invalid user name or password");
        }
        return Ok("user logged in");
    }

}

