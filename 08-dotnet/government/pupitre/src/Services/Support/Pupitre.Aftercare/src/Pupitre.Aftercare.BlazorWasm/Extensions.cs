using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Aftercare.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddAftercareBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAftercareBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

