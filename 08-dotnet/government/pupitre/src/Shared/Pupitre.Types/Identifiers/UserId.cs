namespace Pupitre.Types.Identifiers;

/// <summary>
/// Strongly-typed identifier for User entities.
/// </summary>
public sealed record UserId
{
    public Guid Value { get; }

    public UserId() : this(Guid.NewGuid()) { }
    public UserId(Guid value) => Value = value;

    public static UserId New() => new(Guid.NewGuid());
    public static UserId Empty => new(Guid.Empty);

    public static implicit operator UserId(Guid value) => new(value);
    public static implicit operator Guid(UserId id) => id.Value;

    public override string ToString() => Value.ToString();
}
