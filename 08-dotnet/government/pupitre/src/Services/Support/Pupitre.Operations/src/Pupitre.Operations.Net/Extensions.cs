using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Operations.Net;

public static class Extensions
{
    public static IMameyBuilder AddOperationsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseOperationsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

