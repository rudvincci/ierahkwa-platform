using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mamey.Auth.DecentralizedIdentifiers.Audit;

/// <summary>
/// Interface for storing audit events
/// </summary>
public interface IAuditEventStore
{
    Task StoreAsync(AuditEvent auditEvent);
    Task<IEnumerable<AuditEvent>> GetEventsForDidAsync(string did, int limit = 100);
    Task<IEnumerable<AuditEvent>> GetEventsAsync(DateTime from, DateTime to, int limit = 1000);
    Task<IEnumerable<AuditEvent>> GetEventsByTypeAsync(string eventType, int limit = 100);
    Task<AuditStatistics> GetStatisticsAsync(DateTime from, DateTime to);
    Task<AuditSearchResult> SearchEventsAsync(AuditSearchRequest request);
}
