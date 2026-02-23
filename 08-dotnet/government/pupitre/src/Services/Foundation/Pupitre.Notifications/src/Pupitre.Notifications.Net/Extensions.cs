using Mamey;
﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Notifications.Net;

public static class Extensions
{
    public static IMameyBuilder AddNotificationsNet(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseNotificationsNet(this IApplicationBuilder builder)
    {
        return builder;
    }
}

