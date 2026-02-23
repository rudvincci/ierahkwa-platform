using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.AITranslation.Application.Commands;
using Pupitre.AITranslation.Application.DTO;
using Pupitre.AITranslation.Application.Queries;
using Pupitre.AITranslation.Application.Services;
using Pupitre.AITranslation.Contracts.Commands;
using Pupitre.AITranslation.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.AITranslation.Api;

public static class TranslationRequestRoutes
{
    public static IApplicationBuilder AddTranslationRequestRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAITranslation, PagedResult<TranslationRequestDto>?>($"translations", auth: false)
            .Get<GetTranslationRequest, TranslationRequestDetailsDto>($"translations/{{id:guid}}", auth: false)
            .Post<AddTranslationRequest>($"translations",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"translations/{cmd.Id}")!, auth: false)
            .Put<UpdateTranslationRequest>($"translations/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteTranslationRequest>($"translations/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
