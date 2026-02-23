using Mamey.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Mamey.Net.Http;

public static class HttpContextExtensions
{
    public static Guid GetOrganizationHttpHeader(HttpContext context)
    {
        StringValues orgHeader;
        if (!context.Request.Headers.TryGetValue("X-ORG", out orgHeader))
        {
            throw new MissingOrganizationHeaderException();
        }
        return Guid.Parse(orgHeader);
    }
}

