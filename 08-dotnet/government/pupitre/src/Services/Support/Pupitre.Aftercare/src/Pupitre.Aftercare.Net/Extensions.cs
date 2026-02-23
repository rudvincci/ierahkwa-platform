using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Aftercare.Net;

public static class Extensions
{
    public static IMameyBuilder AddAftercareNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAftercareNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

