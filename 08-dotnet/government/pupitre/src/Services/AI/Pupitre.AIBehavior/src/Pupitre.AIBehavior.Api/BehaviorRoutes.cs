using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.AIBehavior.Application.Commands;
using Pupitre.AIBehavior.Application.DTO;
using Pupitre.AIBehavior.Application.Queries;
using Pupitre.AIBehavior.Application.Services;
using Pupitre.AIBehavior.Contracts.Commands;
using Pupitre.AIBehavior.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.AIBehavior.Api;

public static class BehaviorRoutes
{
    public static IApplicationBuilder AddBehaviorRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAIBehavior, PagedResult<BehaviorDto>?>($"behaviors", auth: false)
            .Get<GetBehavior, BehaviorDetailsDto>($"behaviors/{{id:guid}}", auth: false)
            .Post<AddBehavior>($"behaviors",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"behaviors/{cmd.Id}")!, auth: false)
            .Put<UpdateBehavior>($"behaviors/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteBehavior>($"behaviors/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
