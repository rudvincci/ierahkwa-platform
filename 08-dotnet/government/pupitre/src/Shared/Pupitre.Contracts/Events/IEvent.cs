namespace Pupitre.Contracts.Events;

/// <summary>
/// Marker interface for all Pupitre domain events.
/// Events represent something that has happened in the system.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// The unique identifier for this event occurrence.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// The timestamp when this event occurred.
    /// </summary>
    DateTime OccurredAt { get; }
}

/// <summary>
/// Base class for Pupitre domain events.
/// </summary>
public abstract record EventBase : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
