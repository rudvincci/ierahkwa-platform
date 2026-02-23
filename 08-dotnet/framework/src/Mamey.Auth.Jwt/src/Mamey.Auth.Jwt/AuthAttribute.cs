using Microsoft.AspNetCore.Authorization;

namespace Mamey.Auth.Jwt;

public class AuthAttribute : AuthorizeAttribute
{
    public AuthAttribute(string scheme, string policy = "") : base(policy)
    {
        AuthenticationSchemes = scheme;
    }
}