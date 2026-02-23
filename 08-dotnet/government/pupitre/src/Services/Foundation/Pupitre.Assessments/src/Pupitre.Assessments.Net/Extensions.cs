using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Assessments.Net;

public static class Extensions
{
    public static IMameyBuilder AddAssessmentsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAssessmentsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

