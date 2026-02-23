using System;
using Microsoft.AspNetCore.Builder;

namespace Mamey.ServiceName.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddServiceNameBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseServiceNameBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

