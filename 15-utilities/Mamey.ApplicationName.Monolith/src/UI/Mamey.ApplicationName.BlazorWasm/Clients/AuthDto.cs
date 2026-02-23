
using Mamey.ApplicationName.BlazorWasm.Configuration;
using Mamey.BlazorWasm.Http;

namespace Mamey.ApplicationName.BlazorWasm.Clients;

public class AuthDto
{
    public AuthDto() { }
    
    public AuthDto(AuthenticatedUser user, JsonWebToken jwt)
        => (User, Jwt) = (user, jwt);
    public AuthenticatedUser? User { get; set; }
    public JsonWebToken? Jwt { get; set; }
}