namespace MameyFramework.Core;

/// <summary>
/// Base entity for all domain entities
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    
    public void MarkAsUpdated(string? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
    
    public void MarkAsDeleted(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;
        
        if (ReferenceEquals(this, other))
            return true;
        
        return Id == other.Id;
    }
    
    public override int GetHashCode() => Id.GetHashCode();
    
    public static bool operator ==(Entity? left, Entity? right) =>
        left?.Equals(right) ?? right is null;
    
    public static bool operator !=(Entity? left, Entity? right) =>
        !(left == right);
}

/// <summary>
/// Aggregate root marker interface
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// Domain event interface
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
    string EventType { get; }
}

/// <summary>
/// Base domain event
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public abstract string EventType { get; }
}
