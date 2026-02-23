using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Lessons.Application.Commands;
using Pupitre.Lessons.Application.DTO;
using Pupitre.Lessons.Application.Queries;
using Pupitre.Lessons.Application.Services;
using Pupitre.Lessons.Contracts.Commands;
using Pupitre.Lessons.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Lessons.Api;

public static class LessonRoutes
{
    public static IApplicationBuilder AddLessonRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))

            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseLessons, PagedResult<LessonDto>?>($"lessons", auth: false)
            .Get<GetLesson, LessonDetailsDto>($"lessons/{{id:guid}}", auth: false)
            .Post<AddLesson>($"lessons",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"lessons/{cmd.Id}")!, auth: false)
            .Put<UpdateLesson>($"lessons/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteLesson>($"lessons/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
