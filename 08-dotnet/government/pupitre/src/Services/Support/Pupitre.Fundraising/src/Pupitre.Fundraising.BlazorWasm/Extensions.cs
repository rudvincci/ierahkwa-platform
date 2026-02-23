using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Fundraising.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddFundraisingBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseFundraisingBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

