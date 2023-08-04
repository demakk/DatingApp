using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingApp.Api.Entities;
using DatingApp.Api.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Api.Services;

public class TokenService : ITokenService
{

    private readonly SymmetricSecurityKey _key;
    
    public TokenService(IConfiguration config)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
    }
    public string CreateToken(AppUser user)
    {

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, user.UserName.ToLower()),
            }),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}