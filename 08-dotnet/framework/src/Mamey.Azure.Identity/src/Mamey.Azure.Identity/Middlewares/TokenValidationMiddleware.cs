using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mamey.Azure.Identity.Middlewares;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private const string Secret = "your_secret_key"; // Replace with your secret key

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        string authorizationHeader = context.Request.Headers["Authorization"];

        // Edge case: Authorization header is missing
        if (string.IsNullOrEmpty(authorizationHeader))
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }

        string[] parts = authorizationHeader.Split(' ');
        // Edge case: Authorization header does not have two parts
        if (parts.Length != 2)
        {
            context.Response.StatusCode = 400; // Bad Request
            return;
        }

        string scheme = parts[0];
        string parameter = parts[1];

        // Edge case: Authorization scheme is not Bearer
        if (!string.Equals(scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        // Edge case: Token is not readable
        if (!tokenHandler.CanReadToken(parameter))
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            tokenHandler.ValidateToken(parameter, validationParameters, out SecurityToken validatedToken);
        }
        catch (SecurityTokenExpiredException)
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }
        catch (SecurityTokenInvalidIssuerException)
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }
        catch (SecurityTokenInvalidAudienceException)
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }
        catch
        {
            context.Response.StatusCode = 500; // Internal Server Error
            return;
        }

        await _next(context);
    }
}