using System;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Compliance.Domain.Entities;

[Serializable]
public class ComplianceRecordId : AggregateId<Guid>, IEquatable<ComplianceRecordId>
{
    public ComplianceRecordId()
        : this(Guid.NewGuid())
    {

    }

    [JsonConstructor]
    public ComplianceRecordId(Guid compliancerecordId) : base(compliancerecordId)
    {
        Value = compliancerecordId;
    }

    [JsonPropertyName("compliancerecordId")]
    public override Guid Value { get; }

    public static implicit operator Guid(ComplianceRecordId id) => id.Value;

    public static implicit operator ComplianceRecordId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(ComplianceRecordId? other)
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
        return Equals((ComplianceRecordId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out ComplianceRecordId result)
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

