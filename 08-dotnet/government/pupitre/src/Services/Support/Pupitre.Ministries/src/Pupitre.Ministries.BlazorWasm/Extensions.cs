using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Ministries.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddMinistriesBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseMinistriesBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

