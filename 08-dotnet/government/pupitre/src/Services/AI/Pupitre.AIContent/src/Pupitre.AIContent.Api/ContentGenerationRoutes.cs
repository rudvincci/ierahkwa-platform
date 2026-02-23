using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.AIContent.Application.Commands;
using Pupitre.AIContent.Application.DTO;
using Pupitre.AIContent.Application.Queries;
using Pupitre.AIContent.Application.Services;
using Pupitre.AIContent.Contracts.Commands;
using Pupitre.AIContent.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.AIContent.Api;

public static class ContentGenerationRoutes
{
    public static IApplicationBuilder AddContentGenerationRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAIContent, PagedResult<ContentGenerationDto>?>($"content", auth: false)
            .Get<GetContentGeneration, ContentGenerationDetailsDto>($"content/{{id:guid}}", auth: false)
            .Post<AddContentGeneration>($"content",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"content/{cmd.Id}")!, auth: false)
            .Put<UpdateContentGeneration>($"content/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteContentGeneration>($"content/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
