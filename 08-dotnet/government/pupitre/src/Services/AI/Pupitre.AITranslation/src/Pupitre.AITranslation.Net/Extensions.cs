using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AITranslation.Net;

public static class Extensions
{
    public static IMameyBuilder AddAITranslationNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAITranslationNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

