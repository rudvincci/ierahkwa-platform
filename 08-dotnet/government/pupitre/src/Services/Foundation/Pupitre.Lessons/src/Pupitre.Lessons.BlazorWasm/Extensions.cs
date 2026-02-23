using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Lessons.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddLessonsBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseLessonsBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

