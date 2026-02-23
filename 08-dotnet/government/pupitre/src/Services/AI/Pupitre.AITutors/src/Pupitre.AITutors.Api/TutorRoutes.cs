using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.AITutors.Application.Commands;
using Pupitre.AITutors.Application.DTO;
using Pupitre.AITutors.Application.Queries;
using Pupitre.AITutors.Application.Services;
using Pupitre.AITutors.Contracts.Commands;
using Pupitre.AITutors.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.AITutors.Api;

public static class TutorRoutes
{
    public static IApplicationBuilder AddTutorRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAITutors, PagedResult<TutorDto>?>($"tutors", auth: false)
            .Get<GetTutor, TutorDetailsDto>($"tutors/{{id:guid}}", auth: false)
            .Post<AddTutor>($"tutors",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"tutors/{cmd.Id}")!, auth: false)
            .Put<UpdateTutor>($"tutors/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteTutor>($"tutors/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
