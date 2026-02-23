using System.Threading.Tasks;

namespace Mamey.Auth.DecentralizedIdentifiers.Audit;

/// <summary>
/// Interface for publishing audit events
/// </summary>
public interface IAuditEventPublisher
{
    Task PublishAsync(AuditEvent auditEvent);
}
