using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIContent.Net;

public static class Extensions
{
    public static IMameyBuilder AddAIContentNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAIContentNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

