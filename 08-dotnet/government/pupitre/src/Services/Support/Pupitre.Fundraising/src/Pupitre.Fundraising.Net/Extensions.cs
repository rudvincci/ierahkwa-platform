using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Fundraising.Net;

public static class Extensions
{
    public static IMameyBuilder AddFundraisingNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseFundraisingNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

