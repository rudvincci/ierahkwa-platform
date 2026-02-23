using System;
using System.Collections.Generic;

namespace Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
    public string TraceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public Guid? TenantId { get; set; }
    public Dictionary<string, object> Headers { get; set; } = new();
}