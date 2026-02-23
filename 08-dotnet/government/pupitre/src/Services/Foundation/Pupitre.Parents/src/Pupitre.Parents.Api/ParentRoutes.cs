using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Parents.Application.Commands;
using Pupitre.Parents.Application.DTO;
using Pupitre.Parents.Application.Queries;
using Pupitre.Parents.Application.Services;
using Pupitre.Parents.Contracts.Commands;
using Pupitre.Parents.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Parents.Api;

public static class ParentRoutes
{
    public static IApplicationBuilder AddParentRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseParents, PagedResult<ParentDto>?>($"parents", auth: false)
            .Get<GetParent, ParentDetailsDto>($"parents/{{id:guid}}", auth: false)
            .Post<AddParent>($"parents",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"parents/{cmd.Id}")!, auth: false)
            .Put<UpdateParent>($"parents/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteParent>($"parents/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
