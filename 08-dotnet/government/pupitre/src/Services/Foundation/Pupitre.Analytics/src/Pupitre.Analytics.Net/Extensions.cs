using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Analytics.Net;

public static class Extensions
{
    public static IMameyBuilder AddAnalyticsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAnalyticsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

