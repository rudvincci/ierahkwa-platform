using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Identity.Net;

public static class Extensions
{
    public static IMameyBuilder AddIdentityNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseIdentityNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

