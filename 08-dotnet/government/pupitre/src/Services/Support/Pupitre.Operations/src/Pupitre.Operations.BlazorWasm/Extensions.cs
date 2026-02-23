using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Operations.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddOperationsBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseOperationsBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

