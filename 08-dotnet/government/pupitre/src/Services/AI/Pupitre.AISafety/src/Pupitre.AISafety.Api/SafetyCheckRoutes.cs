using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.AISafety.Application.Commands;
using Pupitre.AISafety.Application.DTO;
using Pupitre.AISafety.Application.Queries;
using Pupitre.AISafety.Application.Services;
using Pupitre.AISafety.Contracts.Commands;
using Pupitre.AISafety.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.AISafety.Api;

public static class SafetyCheckRoutes
{
    public static IApplicationBuilder AddSafetyCheckRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAISafety, PagedResult<SafetyCheckDto>?>($"safety", auth: false)
            .Get<GetSafetyCheck, SafetyCheckDetailsDto>($"safety/{{id:guid}}", auth: false)
            .Post<AddSafetyCheck>($"safety",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"safety/{cmd.Id}")!, auth: false)
            .Put<UpdateSafetyCheck>($"safety/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteSafetyCheck>($"safety/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
