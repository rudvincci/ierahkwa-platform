namespace Mamey.Types;

/// <summary>
/// Base class for entities within an aggregate boundary.
/// Use <see cref="AggregateRoot{T}"/> for aggregate roots that own domain events and versioning.
/// Use <see cref="Entity{T}"/> for child entities that belong to an aggregate but don't emit events themselves.
/// </summary>
public abstract class Entity<T> : IEntity<T>, IEquatable<Entity<T>>
{
    protected Entity() { }

    protected Entity(T id)
    {
        Id = id;
    }

    [Key]
    public virtual T Id { get; protected set; } = default!;

    public override bool Equals(object? obj)
        => obj is Entity<T> other && Equals(other);

    public bool Equals(Entity<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<T>.Default.Equals(Id, other.Id);
    }

    public override int GetHashCode()
        => Id is not null ? EqualityComparer<T>.Default.GetHashCode(Id) : 0;

    public static bool operator ==(Entity<T>? left, Entity<T>? right)
        => Equals(left, right);

    public static bool operator !=(Entity<T>? left, Entity<T>? right)
        => !Equals(left, right);
}
