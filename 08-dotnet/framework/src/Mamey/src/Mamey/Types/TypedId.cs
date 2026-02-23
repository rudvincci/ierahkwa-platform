namespace Mamey.Types;

/// <summary>
/// Record-based strongly-typed identifier.
/// Use for value-object IDs in domain models.
/// </summary>
public abstract record TypedId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;

    public static implicit operator Guid(TypedId id) => id.Value;

    public override string ToString() => Value.ToString();
}
