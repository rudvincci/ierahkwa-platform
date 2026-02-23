using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for BehavioralBiometrics entities.
/// </summary>
internal class BehavioralBiometricsId : AggregateId<Guid>, IEquatable<BehavioralBiometricsId>
{
    public BehavioralBiometricsId()
        : this(Guid.NewGuid())
    {
    }

    public BehavioralBiometricsId(Guid behavioralBiometricsId) : base(behavioralBiometricsId)
    {
        Value = behavioralBiometricsId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(BehavioralBiometricsId id) => id.Value;

    public static implicit operator BehavioralBiometricsId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(BehavioralBiometricsId? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(Value, other.Value);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((BehavioralBiometricsId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out BehavioralBiometricsId result)
    {
        Guid result2;
        if (Guid.TryParse(input, out result2))
        {
            result = result2;
            return true;
        }
        result = default!;
        return false;
    }
}
