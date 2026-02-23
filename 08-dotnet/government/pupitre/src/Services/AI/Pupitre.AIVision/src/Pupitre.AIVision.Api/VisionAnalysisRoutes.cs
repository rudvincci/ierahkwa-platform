using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.AIVision.Application.Commands;
using Pupitre.AIVision.Application.DTO;
using Pupitre.AIVision.Application.Queries;
using Pupitre.AIVision.Application.Services;
using Pupitre.AIVision.Contracts.Commands;
using Pupitre.AIVision.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.AIVision.Api;

public static class VisionAnalysisRoutes
{
    public static IApplicationBuilder AddVisionAnalysisRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAIVision, PagedResult<VisionAnalysisDto>?>($"vision", auth: false)
            .Get<GetVisionAnalysis, VisionAnalysisDetailsDto>($"vision/{{id:guid}}", auth: false)
            .Post<AddVisionAnalysis>($"vision",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"vision/{cmd.Id}")!, auth: false)
            .Put<UpdateVisionAnalysis>($"vision/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteVisionAnalysis>($"vision/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
