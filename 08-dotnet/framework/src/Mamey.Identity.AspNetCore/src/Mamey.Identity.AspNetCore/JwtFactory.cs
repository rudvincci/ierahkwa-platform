using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Mamey.Identity.AspNetCore;

public class JwtFactory
{
    private readonly IConfiguration _configuration;
    private readonly SigningCredentials _signingCredentials;

    public JwtFactory(IConfiguration configuration, SigningCredentials signingCredentials)
    {
        _configuration = configuration;
        _signingCredentials = signingCredentials;
    }

    public string CreateToken(ClaimsPrincipal user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(user.Claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = _signingCredentials
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
