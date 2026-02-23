using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mamey.Auth.Identity.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Mamey.Auth.Identity;

/// <summary>
/// Generates JWTs with configured signing credentials and claims.
/// </summary>
public class JwtFactory
{
    private readonly SigningCredentials _creds;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtFactory(SigningCredentials creds, AuthOptions options)
    {
        _issuer = options.Issuer;
        _audience = options.Audience;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.IssuerSigningKey));
        _creds =  new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateToken(IEnumerable<Claim> claims, DateTime expires)
    {
        var jwt = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: _creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}