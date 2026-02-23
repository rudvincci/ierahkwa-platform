using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Educators.Application.Commands;
using Pupitre.Educators.Application.DTO;
using Pupitre.Educators.Application.Queries;
using Pupitre.Educators.Application.Services;
using Pupitre.Educators.Contracts.Commands;
using Pupitre.Educators.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Educators.Api;

public static class EducatorRoutes
{
    public static IApplicationBuilder AddEducatorRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseEducators, PagedResult<EducatorDto>?>($"educators", auth: false)
            .Get<GetEducator, EducatorDetailsDto>($"educators/{{id:guid}}", auth: false)
            .Post<AddEducator>($"educators",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"educators/{cmd.Id}")!, auth: false)
            .Put<UpdateEducator>($"educators/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteEducator>($"educators/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
