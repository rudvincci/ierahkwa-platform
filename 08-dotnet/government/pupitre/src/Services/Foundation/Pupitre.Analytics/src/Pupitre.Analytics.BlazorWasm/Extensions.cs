using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Analytics.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddAnalyticsBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAnalyticsBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

