using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Strongly-typed identifier for DocumentScan entities.
/// </summary>
internal class DocumentScanId : AggregateId<Guid>, IEquatable<DocumentScanId>
{
    public DocumentScanId()
        : this(Guid.NewGuid())
    {
    }

    public DocumentScanId(Guid documentScanId) : base(documentScanId)
    {
        Value = documentScanId;
    }

    public override Guid Value { get; }

    public static implicit operator Guid(DocumentScanId id) => id.Value;

    public static implicit operator DocumentScanId(Guid id) => new(id);

    public bool IsEmpty => Value == Guid.Empty;

    public bool Equals(DocumentScanId? other)
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
        return Equals((DocumentScanId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool TryParse(ReadOnlySpan<char> input, out DocumentScanId result)
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
