using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Bookstore.Application.Commands;
using Pupitre.Bookstore.Application.DTO;
using Pupitre.Bookstore.Application.Queries;
using Pupitre.Bookstore.Application.Services;
using Pupitre.Bookstore.Contracts.Commands;
using Pupitre.Bookstore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Bookstore.Api;

public static class BookRoutes
{
    public static IApplicationBuilder AddBookRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseBookstore, PagedResult<BookDto>?>($"books", auth: false)
            .Get<GetBook, BookDetailsDto>($"books/{{id:guid}}", auth: false)
            .Post<AddBook>($"books",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"books/{cmd.Id}")!, auth: false)
            .Put<UpdateBook>($"books/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteBook>($"books/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
