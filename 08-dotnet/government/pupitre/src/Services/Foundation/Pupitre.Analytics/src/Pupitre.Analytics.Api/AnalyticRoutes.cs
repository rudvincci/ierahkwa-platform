using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Analytics.Application.Commands;
using Pupitre.Analytics.Application.DTO;
using Pupitre.Analytics.Application.Queries;
using Pupitre.Analytics.Application.Services;
using Pupitre.Analytics.Contracts.Commands;
using Pupitre.Analytics.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Analytics.Api;

public static class AnalyticRoutes
{
    public static IApplicationBuilder AddAnalyticRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAnalytics, PagedResult<AnalyticDto>?>($"analytics", auth: false)
            .Get<GetAnalytic, AnalyticDetailsDto>($"analytics/{{id:guid}}", auth: false)
            .Post<AddAnalytic>($"analytics",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"analytics/{cmd.Id}")!, auth: false)
            .Put<UpdateAnalytic>($"analytics/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteAnalytic>($"analytics/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
