using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Rewards.Net;

public static class Extensions
{
    public static IMameyBuilder AddRewardsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseRewardsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

