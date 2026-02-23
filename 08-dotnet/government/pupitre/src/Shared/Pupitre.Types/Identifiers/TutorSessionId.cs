namespace Pupitre.Types.Identifiers;

/// <summary>
/// Strongly-typed identifier for AI Tutor Session entities.
/// </summary>
public sealed record TutorSessionId
{
    public Guid Value { get; }

    public TutorSessionId() : this(Guid.NewGuid()) { }
    public TutorSessionId(Guid value) => Value = value;

    public static TutorSessionId New() => new(Guid.NewGuid());
    public static TutorSessionId Empty => new(Guid.Empty);

    public static implicit operator TutorSessionId(Guid value) => new(value);
    public static implicit operator Guid(TutorSessionId id) => id.Value;

    public override string ToString() => Value.ToString();
}
