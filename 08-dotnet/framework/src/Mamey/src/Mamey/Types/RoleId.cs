namespace Mamey.Types;

/// <summary>
/// Represents a role's unique identifier.
/// </summary>
[Serializable]
public class RoleId : AggregateId<Guid>, IEquatable<RoleId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleId"/> class with a new GUID.
    /// </summary>
    public RoleId()
        : this(Guid.NewGuid())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleId"/> class with the specified GUID.
    /// </summary>
    /// <param name="id">The GUID value of the role ID.</param>

    [JsonConstructor]
    public RoleId(Guid id) : base(id)
    {
        Value = id;
    }

    /// <summary>
    /// Gets the GUID value of the role ID.
    /// </summary>
    [JsonPropertyName("id")]
    public override Guid Value { get; }


    /// <summary>
    /// Implicit conversion from <see cref="RoleId"/> to <see cref="Guid"/>.
    /// </summary>
    /// <param name="id">The role ID.</param>
    public static implicit operator Guid(RoleId id) => id?.Value ?? Guid.Empty;

    /// <summary>
    /// Implicit conversion from <see cref="Guid"/> to <see cref="RoleId"/>.
    /// </summary>
    /// <param name="id">The GUID value.</param>
    public static implicit operator RoleId(Guid id) => new(id);

    /// <summary>
    /// Determines whether the role ID is empty.
    /// </summary>
    public bool IsEmpty => Value == Guid.Empty;

    /// <inheritdoc />
    public bool Equals(RoleId? other)
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
        return obj is RoleId other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    public static RoleId New() => new RoleId(Guid.NewGuid());

    /// <summary>
    /// Tries to parse a string representation of a GUID to a <see cref="RoleId"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="result">The parsed role ID.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public static bool TryParse(ReadOnlySpan<char> input, out RoleId? result)
    {
        if (Guid.TryParse(input, out Guid result2))
        {
            result = new RoleId(result2);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Addition operator for <see cref="RoleId"/> objects.
    /// </summary>
    /// <param name="a">The first role ID.</param>
    /// <param name="b">The second role ID.</param>
    /// <returns>A new role ID with the combined GUID value.</returns>
    public static RoleId operator +(RoleId a, RoleId b) => new RoleId(CombineGuids(a.Value, b.Value));

    /// <summary>
    /// Equality operator for <see cref="RoleId"/> objects.
    /// </summary>
    /// <param name="a">The first role ID.</param>
    /// <param name="b">The second role ID.</param>
    /// <returns>True if the values are equal; otherwise, false.</returns>
    public static bool operator ==(RoleId a, RoleId b) => a?.Equals(b) ?? false;

    /// <summary>
    /// Inequality operator for <see cref="RoleId"/> objects.
    /// </summary>
    /// <param name="a">The first role ID.</param>
    /// <param name="b">The second role ID.</param>
    /// <returns>True if the values are not equal; otherwise, false.</returns>
    public static bool operator !=(RoleId a, RoleId b) => !(a == b);

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