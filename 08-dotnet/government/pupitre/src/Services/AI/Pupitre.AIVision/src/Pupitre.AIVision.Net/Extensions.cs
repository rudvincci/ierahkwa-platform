using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIVision.Net;

public static class Extensions
{
    public static IMameyBuilder AddAIVisionNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAIVisionNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

