using System.Security.Cryptography;
using System.Text;
using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers;

public class AccountController : BaseApiController
{
    private readonly DataContext _ctx;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext ctx, ITokenService tokenService)
    {
        _ctx = ctx;
        _tokenService = tokenService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterDTO registerDto)
    {
        if (await UserExists(registerDto.UserName))
        {
            return BadRequest("UserName is taken");
        }
        
        using var hmac = new HMACSHA512(); //HMACSHA512 class derives from IDisposable
        var user = new AppUser
        {
            UserName = registerDto.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        await _ctx.Users.AddAsync(user);

        await _ctx.SaveChangesAsync();
        
        return Ok(new UserDTO{UserName = user.UserName, Token = _tokenService.CreateToken(user)});
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginDTO login)
    {
        var user = await _ctx.Users
            .FirstOrDefaultAsync(i => i.UserName == login.UserName.ToLower());

        if (user is null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return Ok(new UserDTO{UserName = user.UserName, Token = _tokenService.CreateToken(user)});
    }

    private async Task<bool> UserExists(string username)
    {
        return await _ctx.Users.AnyAsync(u => u.UserName == username.ToLower());
    }



}