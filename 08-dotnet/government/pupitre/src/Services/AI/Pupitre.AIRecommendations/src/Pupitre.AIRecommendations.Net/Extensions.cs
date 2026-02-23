using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIRecommendations.Net;

public static class Extensions
{
    public static IMameyBuilder AddAIRecommendationsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAIRecommendationsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

