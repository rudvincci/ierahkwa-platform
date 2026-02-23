using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Ministries.Net;

public static class Extensions
{
    public static IMameyBuilder AddMinistriesNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseMinistriesNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

