using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Compliance.Net;

public static class Extensions
{
    public static IMameyBuilder AddComplianceNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseComplianceNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

