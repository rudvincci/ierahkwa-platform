using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AISpeech.Net;

public static class Extensions
{
    public static IMameyBuilder AddAISpeechNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAISpeechNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

