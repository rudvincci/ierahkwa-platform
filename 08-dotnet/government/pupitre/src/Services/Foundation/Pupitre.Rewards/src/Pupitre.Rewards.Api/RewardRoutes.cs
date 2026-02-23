using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Rewards.Application.Commands;
using Pupitre.Rewards.Application.DTO;
using Pupitre.Rewards.Application.Queries;
using Pupitre.Rewards.Application.Services;
using Pupitre.Rewards.Contracts.Commands;
using Pupitre.Rewards.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Rewards.Api;

public static class RewardRoutes
{
    public static IApplicationBuilder AddRewardRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseRewards, PagedResult<RewardDto>?>($"rewards", auth: false)
            .Get<GetReward, RewardDetailsDto>($"rewards/{{id:guid}}", auth: false)
            .Post<AddReward>($"rewards",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"rewards/{cmd.Id}")!, auth: false)
            .Put<UpdateReward>($"rewards/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteReward>($"rewards/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
