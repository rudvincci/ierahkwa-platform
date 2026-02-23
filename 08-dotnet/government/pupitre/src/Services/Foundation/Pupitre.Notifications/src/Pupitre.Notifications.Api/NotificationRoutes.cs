using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Notifications.Application.Commands;
using Pupitre.Notifications.Application.DTO;
using Pupitre.Notifications.Application.Queries;
using Pupitre.Notifications.Application.Services;
using Pupitre.Notifications.Contracts.Commands;
using Pupitre.Notifications.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Notifications.Api;

public static class NotificationRoutes
{
    public static IApplicationBuilder AddNotificationRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseNotifications, PagedResult<NotificationDto>?>($"notifications", auth: false)
            .Get<GetNotification, NotificationDetailsDto>($"notifications/{{id:guid}}", auth: false)
            .Post<AddNotification>($"notifications",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"notifications/{cmd.Id}")!, auth: false)
            .Put<UpdateNotification>($"notifications/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteNotification>($"notifications/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
