using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AISafety.Net;

public static class Extensions
{
    public static IMameyBuilder AddAISafetyNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAISafetyNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

