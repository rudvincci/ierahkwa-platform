using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Application.Commands;
using Mamey.FWID.Notifications.Application.DTO;
using Mamey.FWID.Notifications.Application.Queries;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace Mamey.FWID.Notifications.Api;

/// <summary>
/// Notification service API routes.
/// </summary>
public static class NotificationRoutes
{
    /// <summary>
    /// Maps notification service routes.
    /// </summary>
    public static IApplicationBuilder AddNotificationRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
        );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            // Get Notifications (GET /api/notifications/{identityId})
            .Get<GetNotifications, List<NotificationDto>>("/api/notifications/{identityId:guid}",
                beforeDispatch: (query, ctx) =>
                {
                    // Extract IdentityId from route and set in query using backing field
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) && 
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        // Use reflection to set backing field (records use compiler-generated fields)
                        var field = typeof(GetNotifications).GetFields(
                            System.Reflection.BindingFlags.Instance | 
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(query, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (query, result, ctx) => ctx.Response.WriteJsonAsync(result),
                auth: false)
            
            // Get All Notifications (GET /api/notifications)
            .Get<GetNotifications, List<NotificationDto>>("/api/notifications",
                afterDispatch: (query, result, ctx) => ctx.Response.WriteJsonAsync(result),
                auth: false)
            
            // Add Notification (POST /api/notifications)
            .Post<AddNotification>("/api/notifications",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => 
                {
                    ctx.Response.StatusCode = 201;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Notification created successfully", id = cmd.Id });
                }, 
                auth: false)
            
            // Mark Notification as Read (POST /api/notifications/{notificationId}/read)
            .Post<MarkAsRead>("/api/notifications/{notificationId:guid}/read",
                beforeDispatch: ([FromBody] cmd, ctx) =>
                {
                    // Extract NotificationId and IdentityId from route and set in command using backing field
                    if (ctx.Request.RouteValues.TryGetValue("notificationId", out var notificationIdValue) && 
                        Guid.TryParse(notificationIdValue?.ToString(), out var notificationIdGuid))
                    {
                        var notificationIdField = typeof(MarkAsRead).GetFields(
                            System.Reflection.BindingFlags.Instance | 
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("NotificationId", StringComparison.OrdinalIgnoreCase));
                        
                        if (notificationIdField != null)
                        {
                            notificationIdField.SetValue(cmd, notificationIdGuid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Notification marked as read successfully" });
                },
                auth: false)
        );
        
        return app;
    }
}

