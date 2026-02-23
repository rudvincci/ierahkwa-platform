using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Operations.Application.Commands;
using Pupitre.Operations.Application.DTO;
using Pupitre.Operations.Application.Queries;
using Pupitre.Operations.Application.Services;
using Pupitre.Operations.Contracts.Commands;
using Pupitre.Operations.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Operations.Api;

public static class OperationMetricRoutes
{
    public static IApplicationBuilder AddOperationMetricRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseOperations, PagedResult<OperationMetricDto>?>($"operations", auth: false)
            .Get<GetOperationMetric, OperationMetricDetailsDto>($"operations/{{id:guid}}", auth: false)
            .Post<AddOperationMetric>($"operations",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"operations/{cmd.Id}")!, auth: false)
            .Put<UpdateOperationMetric>($"operations/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteOperationMetric>($"operations/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
