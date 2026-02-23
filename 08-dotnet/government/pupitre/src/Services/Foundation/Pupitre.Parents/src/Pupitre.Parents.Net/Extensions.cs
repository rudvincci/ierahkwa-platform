using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Parents.Net;

public static class Extensions
{
    public static IMameyBuilder AddParentsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseParentsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

