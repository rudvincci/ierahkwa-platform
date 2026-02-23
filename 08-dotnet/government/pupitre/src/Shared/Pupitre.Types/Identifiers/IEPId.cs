namespace Pupitre.Types.Identifiers;

/// <summary>
/// Strongly-typed identifier for Individualized Education Plan (IEP) entities.
/// </summary>
public sealed record IEPId
{
    public Guid Value { get; }

    public IEPId() : this(Guid.NewGuid()) { }
    public IEPId(Guid value) => Value = value;

    public static IEPId New() => new(Guid.NewGuid());
    public static IEPId Empty => new(Guid.Empty);

    public static implicit operator IEPId(Guid value) => new(value);
    public static implicit operator Guid(IEPId id) => id.Value;

    public override string ToString() => Value.ToString();
}
