using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Educators.Net;

public static class Extensions
{
    public static IMameyBuilder AddEducatorsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseEducatorsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

