using System;
using Microsoft.AspNetCore.Builder;

namespace Pupitre.Notifications.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddNotificationsBlazorWasm(this IMameyBuilder builder)
    {
        return builder;
    }


    public static IApplicationBuilder UseNotificationsBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

