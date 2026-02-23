using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIAssessments.Net;

public static class Extensions
{
    public static IMameyBuilder AddAIAssessmentsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAIAssessmentsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

