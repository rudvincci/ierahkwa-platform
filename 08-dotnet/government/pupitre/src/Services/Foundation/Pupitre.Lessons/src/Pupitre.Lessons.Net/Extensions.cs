using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Lessons.Net;

public static class Extensions
{
    public static IMameyBuilder AddLessonsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseLessonsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

