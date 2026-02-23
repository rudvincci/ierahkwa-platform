using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Progress.Application.Commands;
using Pupitre.Progress.Application.DTO;
using Pupitre.Progress.Application.Queries;
using Pupitre.Progress.Application.Services;
using Pupitre.Progress.Contracts.Commands;
using Pupitre.Progress.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Progress.Api;

public static class LearningProgressRoutes
{
    public static IApplicationBuilder AddLearningProgressRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseProgress, PagedResult<LearningProgressDto>?>($"progress", auth: false)
            .Get<GetLearningProgress, LearningProgressDetailsDto>($"progress/{{id:guid}}", auth: false)
            .Post<AddLearningProgress>($"progress",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"progress/{cmd.Id}")!, auth: false)
            .Put<UpdateLearningProgress>($"progress/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteLearningProgress>($"progress/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
