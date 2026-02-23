using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AITutors.Net;

public static class Extensions
{
    public static IMameyBuilder AddAITutorsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAITutorsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

