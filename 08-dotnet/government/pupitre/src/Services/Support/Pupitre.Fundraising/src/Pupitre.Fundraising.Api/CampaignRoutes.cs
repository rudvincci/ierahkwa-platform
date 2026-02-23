using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Fundraising.Application.Commands;
using Pupitre.Fundraising.Application.DTO;
using Pupitre.Fundraising.Application.Queries;
using Pupitre.Fundraising.Application.Services;
using Pupitre.Fundraising.Contracts.Commands;
using Pupitre.Fundraising.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Fundraising.Api;

public static class CampaignRoutes
{
    public static IApplicationBuilder AddCampaignRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseFundraising, PagedResult<CampaignDto>?>($"campaigns", auth: false)
            .Get<GetCampaign, CampaignDetailsDto>($"campaigns/{{id:guid}}", auth: false)
            .Post<AddCampaign>($"campaigns",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"campaigns/{cmd.Id}")!, auth: false)
            .Put<UpdateCampaign>($"campaigns/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteCampaign>($"campaigns/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
