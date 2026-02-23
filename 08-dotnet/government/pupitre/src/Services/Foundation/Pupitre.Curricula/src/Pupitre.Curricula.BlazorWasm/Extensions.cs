using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Curricula.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddCurriculaBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseCurriculaBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

