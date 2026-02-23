using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Users.Net;

public static class Extensions
{
    public static IMameyBuilder AddUsersNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseUsersNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

