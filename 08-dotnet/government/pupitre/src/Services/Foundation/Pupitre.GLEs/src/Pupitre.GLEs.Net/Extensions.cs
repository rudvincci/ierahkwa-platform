using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.GLEs.Net;

public static class Extensions
{
    public static IMameyBuilder AddGLEsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseGLEsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

