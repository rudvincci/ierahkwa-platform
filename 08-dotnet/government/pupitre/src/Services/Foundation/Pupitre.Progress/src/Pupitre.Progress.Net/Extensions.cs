using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Progress.Net;

public static class Extensions
{
    public static IMameyBuilder AddProgressNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseProgressNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

