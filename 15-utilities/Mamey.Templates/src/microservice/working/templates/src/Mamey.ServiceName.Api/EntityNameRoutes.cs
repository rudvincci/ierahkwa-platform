using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Mamey.ServiceName.Application.Commands;
using Mamey.ServiceName.Application.DTO;
using Mamey.ServiceName.Application.Queries;
using Mamey.ServiceName.Application.Services;
using Mamey.ServiceName.Contracts.Commands;
using Mamey.ServiceName.Domain.Entities;
using Mamey.Types;
using Microsoft.AspNetCore.Mvc;

namespace Mamey.ServiceName.Api;

public static class EntityNameRoutes
{
    public static IApplicationBuilder AddEntityNameRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))

            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseServiceName, PagedResult<EntityNameDto>?>($"servicename", auth: false)
            .Get<GetEntityName, EntityNameDetailsDto>($"servicename/{{id:guid}}", auth: false)
            .Post<AddEntityName>($"servicename",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"servicename/{cmd.Id}"), auth: false)
            .Put<UpdateEntityName>($"servicename/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Delete<DeleteEntityName>($"servicename/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
        );
        return app;
    }
}

