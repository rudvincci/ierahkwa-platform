using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Aftercare.Application.Commands;
using Pupitre.Aftercare.Application.DTO;
using Pupitre.Aftercare.Application.Queries;
using Pupitre.Aftercare.Application.Services;
using Pupitre.Aftercare.Contracts.Commands;
using Pupitre.Aftercare.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Aftercare.Api;

public static class AftercarePlanRoutes
{
    public static IApplicationBuilder AddAftercarePlanRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAftercare, PagedResult<AftercarePlanDto>?>($"aftercare", auth: false)
            .Get<GetAftercarePlan, AftercarePlanDetailsDto>($"aftercare/{{id:guid}}", auth: false)
            .Post<AddAftercarePlan>($"aftercare",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"aftercare/{cmd.Id}")!, auth: false)
            .Put<UpdateAftercarePlan>($"aftercare/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteAftercarePlan>($"aftercare/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
