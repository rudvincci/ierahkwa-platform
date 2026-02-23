namespace Pupitre.Types.Identifiers;

/// <summary>
/// Strongly-typed identifier for Grade-Level Expectation (GLE) entities.
/// </summary>
public sealed record GLEId
{
    public Guid Value { get; }

    public GLEId() : this(Guid.NewGuid()) { }
    public GLEId(Guid value) => Value = value;

    public static GLEId New() => new(Guid.NewGuid());
    public static GLEId Empty => new(Guid.Empty);

    public static implicit operator GLEId(Guid value) => new(value);
    public static implicit operator Guid(GLEId id) => id.Value;

    public override string ToString() => Value.ToString();
}
