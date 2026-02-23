namespace Pupitre.Types.Identifiers;

/// <summary>
/// Strongly-typed identifier for Reward entities.
/// </summary>
public sealed record RewardId
{
    public Guid Value { get; }

    public RewardId() : this(Guid.NewGuid()) { }
    public RewardId(Guid value) => Value = value;

    public static RewardId New() => new(Guid.NewGuid());
    public static RewardId Empty => new(Guid.Empty);

    public static implicit operator RewardId(Guid value) => new(value);
    public static implicit operator Guid(RewardId id) => id.Value;

    public override string ToString() => Value.ToString();
}
