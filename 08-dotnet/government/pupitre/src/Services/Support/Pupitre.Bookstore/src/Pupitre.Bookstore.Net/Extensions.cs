using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Bookstore.Net;

public static class Extensions
{
    public static IMameyBuilder AddBookstoreNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseBookstoreNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

