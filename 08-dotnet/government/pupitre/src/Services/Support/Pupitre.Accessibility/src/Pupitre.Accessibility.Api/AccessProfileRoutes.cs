using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Accessibility.Application.Commands;
using Pupitre.Accessibility.Application.DTO;
using Pupitre.Accessibility.Application.Queries;
using Pupitre.Accessibility.Application.Services;
using Pupitre.Accessibility.Contracts.Commands;
using Pupitre.Accessibility.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Accessibility.Api;

public static class AccessProfileRoutes
{
    public static IApplicationBuilder AddAccessProfileRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAccessibility, PagedResult<AccessProfileDto>?>($"accessibility", auth: false)
            .Get<GetAccessProfile, AccessProfileDetailsDto>($"accessibility/{{id:guid}}", auth: false)
            .Post<AddAccessProfile>($"accessibility",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"accessibility/{cmd.Id}")!, auth: false)
            .Put<UpdateAccessProfile>($"accessibility/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteAccessProfile>($"accessibility/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
