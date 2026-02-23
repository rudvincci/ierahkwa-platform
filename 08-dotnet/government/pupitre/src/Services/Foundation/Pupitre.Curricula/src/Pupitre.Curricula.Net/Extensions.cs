using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Curricula.Net;

public static class Extensions
{
    public static IMameyBuilder AddCurriculaNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseCurriculaNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

