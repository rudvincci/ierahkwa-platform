namespace Pupitre.Types.Identifiers;

/// <summary>
/// Strongly-typed identifier for Assessment entities.
/// </summary>
public sealed record AssessmentId
{
    public Guid Value { get; }

    public AssessmentId() : this(Guid.NewGuid()) { }
    public AssessmentId(Guid value) => Value = value;

    public static AssessmentId New() => new(Guid.NewGuid());
    public static AssessmentId Empty => new(Guid.Empty);

    public static implicit operator AssessmentId(Guid value) => new(value);
    public static implicit operator Guid(AssessmentId id) => id.Value;

    public override string ToString() => Value.ToString();
}
