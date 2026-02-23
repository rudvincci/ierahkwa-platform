namespace Pupitre.Types.Identifiers;

/// <summary>
/// Strongly-typed identifier for Curriculum entities.
/// </summary>
public sealed record CurriculumId
{
    public Guid Value { get; }

    public CurriculumId() : this(Guid.NewGuid()) { }
    public CurriculumId(Guid value) => Value = value;

    public static CurriculumId New() => new(Guid.NewGuid());
    public static CurriculumId Empty => new(Guid.Empty);

    public static implicit operator CurriculumId(Guid value) => new(value);
    public static implicit operator Guid(CurriculumId id) => id.Value;

    public override string ToString() => Value.ToString();
}
