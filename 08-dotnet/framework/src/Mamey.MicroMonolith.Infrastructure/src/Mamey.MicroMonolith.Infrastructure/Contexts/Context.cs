using System;
using Microsoft.AspNetCore.Http;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Mamey.MicroMonolith.Infrastructure.Api;

namespace Mamey.MicroMonolith.Infrastructure.Contexts;

public class Context : IContext
{
    public Guid RequestId { get; } = Guid.NewGuid();
    public Guid CorrelationId { get; }
    public string TraceId { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
    public Guid? TenantId { get; }// TODO: need to get from X-TenantId header value
    public IIdentityContext Identity { get; }

    public Context() : this(Guid.NewGuid(), $"{Guid.NewGuid():N}", null)
    {
    }

    public Context(HttpContext context) : this(context.TryGetCorrelationId(), context.TraceIdentifier,
        new IdentityContext(context.User), context.GetUserIpAddress(),
        context.Request.Headers["user-agent"], 
        string.IsNullOrEmpty(context.Request.Headers["X-TENANT-ID"]) ? null : Guid.Parse(context.Request.Headers["X-TENANT-ID"]))
    {
    }

    public Context(Guid? correlationId, string traceId, IIdentityContext identity = null, string ipAddress = null,
        string userAgent = null, Guid? tenantId = null)
    {
        CorrelationId = correlationId ?? Guid.NewGuid();
        TraceId = traceId;
        Identity = identity ?? IdentityContext.Empty;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        TenantId = tenantId;
    }

    public static IContext Empty => new Context();
}