namespace Pupitre.Types.Identifiers;

/// <summary>
/// Strongly-typed identifier for Lesson entities.
/// </summary>
public sealed record LessonId
{
    public Guid Value { get; }

    public LessonId() : this(Guid.NewGuid()) { }
    public LessonId(Guid value) => Value = value;

    public static LessonId New() => new(Guid.NewGuid());
    public static LessonId Empty => new(Guid.Empty);

    public static implicit operator LessonId(Guid value) => new(value);
    public static implicit operator Guid(LessonId id) => id.Value;

    public override string ToString() => Value.ToString();
}
