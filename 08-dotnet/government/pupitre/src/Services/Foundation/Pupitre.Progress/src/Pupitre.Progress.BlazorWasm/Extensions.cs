using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Progress.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddProgressBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseProgressBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

