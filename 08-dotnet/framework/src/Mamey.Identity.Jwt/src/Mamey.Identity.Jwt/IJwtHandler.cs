using Mamey.Identity.Core;

namespace Mamey.Identity.Jwt;

public interface IJwtHandler
{
    JsonWebToken CreateToken(string userId, string role = null, string audience = null,
        IDictionary<string, string> claims = null);

    JsonWebTokenPayload GetTokenPayload(string accessToken);
}
