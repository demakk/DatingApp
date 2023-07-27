using DatingApp.Api.Data;
using DatingApp.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace DatingApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly DataContext _ctx;
    
    public UsersController(DataContext ctx)
    {
        _ctx = ctx;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _ctx.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == id); 
        return Ok(user);
    }
}