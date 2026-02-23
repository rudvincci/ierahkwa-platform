namespace Mamey.Types;

/// <summary>
/// Represents a tenant's unique identifier.
/// </summary>
[Serializable]
public class TenantId : AggregateId<Guid>, IEquatable<TenantId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantId"/> class with a new GUID.
    /// </summary>
    public TenantId()
        : this(Guid.NewGuid()) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantId"/> class with the specified GUID.
    /// </summary>
    /// <param name="id">The GUID value of the tenant ID.</param>
 
    [JsonConstructor]
    public TenantId(Guid id) : base(id)
    {
        Value = id;
    }

    /// <summary>
    /// Gets the GUID value of the tenant ID.
    /// </summary>
    [JsonPropertyName("id")]
    public override Guid Value { get; }
    

    /// <summary>
    /// Implicit conversion from <see cref="TenantId"/> to <see cref="Guid"/>.
    /// </summary>
    /// <param name="id">The tenant ID.</param>
    public static implicit operator Guid(TenantId id) => id?.Value ?? Guid.Empty;

    /// <summary>
    /// Implicit conversion from <see cref="Guid"/> to <see cref="TenantId"/>.
    /// </summary>
    /// <param name="id">The GUID value.</param>
    public static implicit operator TenantId(Guid id) => new(id);

    /// <summary>
    /// Determines whether the tenant ID is empty.
    /// </summary>
    public bool IsEmpty => Value == Guid.Empty;

    /// <inheritdoc />
    public bool Equals(TenantId? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value.Equals(other.Value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is TenantId other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
    
    public static TenantId New() => new TenantId(Guid.NewGuid());
    public static TenantId Empty() => new TenantId(Guid.Empty);

    /// <summary>
    /// Tries to parse a string representation of a GUID to a <see cref="TenantId"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="result">The parsed tenant ID.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public static bool TryParse(ReadOnlySpan<char> input, out TenantId? result)
    {
        if (Guid.TryParse(input, out Guid result2))
        {
            result = new TenantId(result2);
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Addition operator for <see cref="TenantId"/> objects.
    /// </summary>
    /// <param name="a">The first tenant ID.</param>
    /// <param name="b">The second tenant ID.</param>
    /// <returns>A new tenant ID with the combined GUID value.</returns>
    public static TenantId operator +(TenantId a, TenantId b) => new TenantId(CombineGuids(a.Value, b.Value));

    /// <summary>
    /// Equality operator for <see cref="TenantId"/> objects.
    /// </summary>
    /// <param name="a">The first tenant ID.</param>
    /// <param name="b">The second tenant ID.</param>
    /// <returns>True if the values are equal; otherwise, false.</returns>
    public static bool operator ==(TenantId a, TenantId b) => a?.Equals(b) ?? false;

    /// <summary>
    /// Inequality operator for <see cref="TenantId"/> objects.
    /// </summary>
    /// <param name="a">The first tenant ID.</param>
    /// <param name="b">The second tenant ID.</param>
    /// <returns>True if the values are not equal; otherwise, false.</returns>
    public static bool operator !=(TenantId a, TenantId b) => !(a == b);

    /// <summary>
    /// Combines two GUIDs to create a new GUID.
    /// </summary>
    /// <param name="a">The first GUID.</param>
    /// <param name="b">The second GUID.</param>
    /// <returns>The combined GUID.</returns>
    private static Guid CombineGuids(Guid a, Guid b)
    {
        byte[] bytesA = a.ToByteArray();
        byte[] bytesB = b.ToByteArray();
        for (int i = 0; i < bytesA.Length; i++)
        {
            bytesA[i] ^= bytesB[i];
        }
        return new Guid(bytesA);
    }
}