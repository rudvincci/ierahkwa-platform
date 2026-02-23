using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Bookstore.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddBookstoreBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseBookstoreBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

