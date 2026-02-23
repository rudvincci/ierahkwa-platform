using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.GLEs.Application.Commands;
using Pupitre.GLEs.Application.DTO;
using Pupitre.GLEs.Application.Queries;
using Pupitre.GLEs.Application.Services;
using Pupitre.GLEs.Contracts.Commands;
using Pupitre.GLEs.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.GLEs.Api;

public static class GLERoutes
{
    public static IApplicationBuilder AddGLERoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))

            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseGLEs, PagedResult<GLEDto>?>($"gles", auth: false)
            .Get<GetGLE, GLEDetailsDto>($"gles/{{id:guid}}", auth: false)
            .Post<AddGLE>($"gles",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"gles/{cmd.Id}")!, auth: false)
            .Put<UpdateGLE>($"gles/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteGLE>($"gles/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}

