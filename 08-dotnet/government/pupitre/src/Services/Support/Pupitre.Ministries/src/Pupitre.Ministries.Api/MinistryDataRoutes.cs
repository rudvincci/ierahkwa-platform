using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Ministries.Application.Commands;
using Pupitre.Ministries.Application.DTO;
using Pupitre.Ministries.Application.Queries;
using Pupitre.Ministries.Application.Services;
using Pupitre.Ministries.Contracts.Commands;
using Pupitre.Ministries.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Ministries.Api;

public static class MinistryDataRoutes
{
    public static IApplicationBuilder AddMinistryDataRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseMinistries, PagedResult<MinistryDataDto>?>($"ministries", auth: false)
            .Get<GetMinistryData, MinistryDataDetailsDto>($"ministries/{{id:guid}}", auth: false)
            .Post<AddMinistryData>($"ministries",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"ministries/{cmd.Id}")!, auth: false)
            .Put<UpdateMinistryData>($"ministries/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteMinistryData>($"ministries/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
