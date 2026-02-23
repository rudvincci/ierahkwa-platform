using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Compliance.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddComplianceBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseComplianceBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

