using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.AIAdaptive.Application.Commands;
using Pupitre.AIAdaptive.Application.DTO;
using Pupitre.AIAdaptive.Application.Queries;
using Pupitre.AIAdaptive.Application.Services;
using Pupitre.AIAdaptive.Contracts.Commands;
using Pupitre.AIAdaptive.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.AIAdaptive.Api;

public static class AdaptiveLearningRoutes
{
    public static IApplicationBuilder AddAdaptiveLearningRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseAIAdaptive, PagedResult<AdaptiveLearningDto>?>($"adaptive", auth: false)
            .Get<GetAdaptiveLearning, AdaptiveLearningDetailsDto>($"adaptive/{{id:guid}}", auth: false)
            .Post<AddAdaptiveLearning>($"adaptive",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"adaptive/{cmd.Id}")!, auth: false)
            .Put<UpdateAdaptiveLearning>($"adaptive/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteAdaptiveLearning>($"adaptive/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
