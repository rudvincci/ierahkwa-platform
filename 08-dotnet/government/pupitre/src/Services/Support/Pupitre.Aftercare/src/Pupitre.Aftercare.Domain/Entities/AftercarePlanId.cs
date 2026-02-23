using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Aftercare.Domain.Entities;

[Serializable]
public class AftercarePlanId : AggregateId<Guid>, IEquatable<AftercarePlanId>
{
    public AftercarePlanId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public AftercarePlanId(Guid aftercareplanId) : base(aftercareplanId)
    {
        Value = aftercareplanId;
    }

    [JsonPropertyName("aftercareplanId")]
    public override Guid Value { get; }

    public static implicit operator Guid(AftercarePlanId id) => id.Value;

    public static implicit operator AftercarePlanId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(AftercarePlanId? other)
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
        return Equals((AftercarePlanId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out AftercarePlanId result)
    {

        Guid result2;
        if (Guid.TryParse(input, out result2))
        {
            result = result2;
            return true;
        }
        result = default(Guid);
        return false;
    }
}

