using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Accessibility.Net;

public static class Extensions
{
    public static IMameyBuilder AddAccessibilityNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseAccessibilityNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

