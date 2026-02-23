using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.AIRecommendations.Application.Commands;
using Pupitre.AIRecommendations.Application.DTO;
using Pupitre.AIRecommendations.Application.Queries;
using Pupitre.AIRecommendations.Application.Services;
using Pupitre.AIRecommendations.Contracts.Commands;
using Pupitre.AIRecommendations.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.AIRecommendations.Api;

public static class AIRecommendationRoutes
{
    public static IApplicationBuilder AddAIRecommendationRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAIRecommendations, PagedResult<AIRecommendationDto>?>($"recommendations", auth: false)
            .Get<GetAIRecommendation, AIRecommendationDetailsDto>($"recommendations/{{id:guid}}", auth: false)
            .Post<AddAIRecommendation>($"recommendations",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"recommendations/{cmd.Id}")!, auth: false)
            .Put<UpdateAIRecommendation>($"recommendations/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteAIRecommendation>($"recommendations/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
