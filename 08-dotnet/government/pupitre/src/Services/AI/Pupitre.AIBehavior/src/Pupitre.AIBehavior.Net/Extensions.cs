using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIBehavior.Net;

public static class Extensions
{
    public static IMameyBuilder AddAIBehaviorNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAIBehaviorNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

