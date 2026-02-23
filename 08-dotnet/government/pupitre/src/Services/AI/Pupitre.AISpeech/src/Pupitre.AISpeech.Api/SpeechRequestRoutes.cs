using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.AISpeech.Application.Commands;
using Pupitre.AISpeech.Application.DTO;
using Pupitre.AISpeech.Application.Queries;
using Pupitre.AISpeech.Application.Services;
using Pupitre.AISpeech.Contracts.Commands;
using Pupitre.AISpeech.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.AISpeech.Api;

public static class SpeechRequestRoutes
{
    public static IApplicationBuilder AddSpeechRequestRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAISpeech, PagedResult<SpeechRequestDto>?>($"speech", auth: false)
            .Get<GetSpeechRequest, SpeechRequestDetailsDto>($"speech/{{id:guid}}", auth: false)
            .Post<AddSpeechRequest>($"speech",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"speech/{cmd.Id}")!, auth: false)
            .Put<UpdateSpeechRequest>($"speech/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteSpeechRequest>($"speech/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
