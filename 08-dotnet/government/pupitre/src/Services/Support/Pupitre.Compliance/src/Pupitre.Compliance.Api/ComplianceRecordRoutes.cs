using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Contexts;
using Mamey.Exceptions;
using Pupitre.Compliance.Application.Commands;
using Pupitre.Compliance.Application.DTO;
using Pupitre.Compliance.Application.Queries;
using Pupitre.Compliance.Application.Services;
using Pupitre.Compliance.Contracts.Commands;
using Pupitre.Compliance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Pupitre.Compliance.Api;

public static class ComplianceRecordRoutes
{
    public static IApplicationBuilder AddComplianceRecordRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get($"/", (ctx) => ctx.Response.WriteAsJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
            );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            .Get<BrowseCompliance, PagedResult<ComplianceRecordDto>?>($"compliance", auth: false)
            .Get<GetComplianceRecord, ComplianceRecordDetailsDto>($"compliance/{{id:guid}}", auth: false)
            .Post<AddComplianceRecord>($"compliance",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"compliance/{cmd.Id}")!, auth: false)
            .Put<UpdateComplianceRecord>($"compliance/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
            .Delete<DeleteComplianceRecord>($"compliance/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted()!, auth: false)
        );
        return app;
    }
}
