using System.Collections.Concurrent;

namespace Mamey.Identity.Decentralized.Utilities;

/// <summary>
/// Captures and records all critical events for DIDs, VCs, and VPs:
/// creation, update, revocation, verification, etc. Extensible to any sink.
/// </summary>
public class AuditTrail
{
    private readonly ConcurrentQueue<AuditEvent> _inMemoryEvents = new();
    private readonly string _filePath;

    /// <summary>
    /// If filePath is provided, logs are appended to that file (JSONL).
    /// </summary>
    public AuditTrail(string filePath = null)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// Records an audit event to the in-memory queue and file (if set).
    /// </summary>
    public async Task LogEventAsync(AuditEvent auditEvent)
    {
        _inMemoryEvents.Enqueue(auditEvent);

        if (!string.IsNullOrWhiteSpace(_filePath))
        {
            var json = System.Text.Json.JsonSerializer.Serialize(auditEvent);
            await File.AppendAllTextAsync(_filePath, json + Environment.NewLine);
        }
    }

    /// <summary>
    /// Returns a snapshot of all in-memory events (non-persistent).
    /// </summary>
    public List<AuditEvent> GetEvents() => new(_inMemoryEvents);

    /// <summary>
    /// Clears the in-memory event queue.
    /// </summary>
    public void ClearEvents()
    {
        while (_inMemoryEvents.TryDequeue(out _))
        {
        }
    }
}