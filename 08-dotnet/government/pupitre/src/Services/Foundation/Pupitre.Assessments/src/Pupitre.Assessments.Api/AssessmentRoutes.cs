using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Assessments.Application.Commands;
using Pupitre.Assessments.Application.DTO;
using Pupitre.Assessments.Application.Queries;
using Pupitre.Assessments.Application.Services;
using Pupitre.Assessments.Contracts.Commands;
using Pupitre.Assessments.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Assessments.Api;

public static class AssessmentRoutes
{
    public static IApplicationBuilder AddAssessmentRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))

            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAssessments, PagedResult<AssessmentDto>?>($"assessments", auth: false)
            .Get<GetAssessment, AssessmentDetailsDto>($"assessments/{{id:guid}}", auth: false)
            .Post<AddAssessment>($"assessments",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"assessments/{cmd.Id}")!, auth: false)
            .Put<UpdateAssessment>($"assessments/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteAssessment>($"assessments/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
