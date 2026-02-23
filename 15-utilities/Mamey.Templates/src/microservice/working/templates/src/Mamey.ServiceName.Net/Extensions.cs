using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ServiceName.Net;

public static class Extensions
{
    public static IMameyBuilder AddServiceNameNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseServiceNameNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

