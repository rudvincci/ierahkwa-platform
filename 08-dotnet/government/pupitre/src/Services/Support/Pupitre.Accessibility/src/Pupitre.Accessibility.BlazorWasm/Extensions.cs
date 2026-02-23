using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Accessibility.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddAccessibilityBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAccessibilityBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

