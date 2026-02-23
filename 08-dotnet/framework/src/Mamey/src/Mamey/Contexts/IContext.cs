using Mamey.Types;

namespace Mamey.Contexts;

public interface IContext
{
    string RequestId { get; }
    IIdentityContext Identity { get; }
    // public OrganizationId OrganizationHeader { get; }
    Guid CorrelationId { get; }
    string TraceId { get; }
    string IpAddress { get; }
    string UserAgent { get; }
}
