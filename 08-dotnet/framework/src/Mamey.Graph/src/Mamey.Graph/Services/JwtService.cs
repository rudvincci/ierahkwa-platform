using Microsoft.IdentityModel.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mamey.Graph.Services;

public class JwtService : IJwtService
{
    public JwtService()
    {
    }

    public List<Claim?> DecodeJwt(string jwt)
    {
        // Enable the JWT SecurityToken handler to show more detailed validation information
        // Useful for debugging purposes during development
        IdentityModelEventSource.ShowPII = true;

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(jwt);

        if (jwtToken == null)
            throw new ArgumentException("Invalid JWT token");

        // Create a dictionary to store the results
        var claims = new List<Claim?>();

        // List of required claims
        string[] requiredClaims = new string[] {
        "iss", "sub", "aud", "exp", "nonce", "iat", "auth_time",
        "oid", "name", "given_name", "family_name", "jobTitle",
        "emails", "tfp", "at_hash", "mbf"
    };

        // Extract claims from the JWT token and check if they exist
        foreach (var claim in requiredClaims)
        {
            // Check if the JWT contains the claim
            var claimValue = jwtToken.Claims.FirstOrDefault(c => c.Type == claim)?.Value;
            if (claimValue != null)
            {
                claims.Add(new Claim(claim, claimValue));
            }
            else
            {
                // Optionally handle the absence of the claim in the token
                claims.Add(new Claim(claim, "Claim not present"));
            }
        }

        return claims;
    }
}
