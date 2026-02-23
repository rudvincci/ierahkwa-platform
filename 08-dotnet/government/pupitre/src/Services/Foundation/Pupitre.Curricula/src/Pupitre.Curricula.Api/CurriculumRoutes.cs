using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Curricula.Application.Commands;
using Pupitre.Curricula.Application.DTO;
using Pupitre.Curricula.Application.Queries;
using Pupitre.Curricula.Application.Services;
using Pupitre.Curricula.Contracts.Commands;
using Pupitre.Curricula.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Curricula.Api;

public static class CurriculumRoutes
{
    public static IApplicationBuilder AddCurriculumRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))

            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseCurricula, PagedResult<CurriculumDto>?>($"curricula", auth: false)
            .Get<GetCurriculum, CurriculumDetailsDto>($"curriculums/{{id:guid}}", auth: false)
            .Post<AddCurriculum>($"curriculums",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"curriculums/{cmd.Id}")!, auth: false)
            .Put<UpdateCurriculum>($"curriculums/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteCurriculum>($"curriculums/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
