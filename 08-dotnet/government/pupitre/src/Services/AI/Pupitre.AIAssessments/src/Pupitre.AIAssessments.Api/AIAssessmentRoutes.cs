using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.AIAssessments.Application.Commands;
using Pupitre.AIAssessments.Application.DTO;
using Pupitre.AIAssessments.Application.Queries;
using Pupitre.AIAssessments.Application.Services;
using Pupitre.AIAssessments.Contracts.Commands;
using Pupitre.AIAssessments.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.AIAssessments.Api;

public static class AIAssessmentRoutes
{
    public static IApplicationBuilder AddAIAssessmentRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAIAssessments, PagedResult<AIAssessmentDto>?>($"assessments", auth: false)
            .Get<GetAIAssessment, AIAssessmentDetailsDto>($"assessments/{{id:guid}}", auth: false)
            .Post<AddAIAssessment>($"assessments",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"assessments/{cmd.Id}")!, auth: false)
            .Put<UpdateAIAssessment>($"assessments/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteAIAssessment>($"assessments/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
