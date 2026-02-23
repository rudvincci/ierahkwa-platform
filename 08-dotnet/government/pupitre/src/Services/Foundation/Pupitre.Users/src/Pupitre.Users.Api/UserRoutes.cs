using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Users.Application.Commands;
using Pupitre.Users.Application.DTO;
using Pupitre.Users.Application.Queries;
using Pupitre.Users.Application.Services;
using Pupitre.Users.Contracts.Commands;
using Pupitre.Users.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Users.Api;

public static class UserRoutes
{
    public static IApplicationBuilder AddUserRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))

            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseUsers, PagedResult<UserDto>?>($"users", auth: false)
            .Get<GetUser, UserDetailsDto>($"users/{{id:guid}}", auth: false)
            .Post<AddUser>($"users",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"users/{cmd.Id}")!, auth: false)
            .Put<UpdateUser>($"users/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteUser>($"users/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
