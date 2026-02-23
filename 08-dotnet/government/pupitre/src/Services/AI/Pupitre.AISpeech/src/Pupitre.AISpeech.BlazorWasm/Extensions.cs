using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.AISpeech.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddAISpeechBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAISpeechBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

