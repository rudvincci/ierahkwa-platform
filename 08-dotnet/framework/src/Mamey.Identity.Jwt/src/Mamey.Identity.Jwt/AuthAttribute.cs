using Microsoft.AspNetCore.Authorization;

namespace Mamey.Identity.Jwt;

public class AuthAttribute : AuthorizeAttribute
{
    public AuthAttribute(string scheme, string policy = "") : base(policy)
    {
        AuthenticationSchemes = scheme;
    }
}
