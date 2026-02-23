namespace Mamey.Types;

/// <summary>
/// Represents a user's unique identifier.
/// </summary>
[Serializable]
public class UserId : AggregateId<Guid>, IEquatable<UserId>
{
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UserId"/> class with a new GUID.
    /// </summary>
    public UserId()
        : this(Guid.NewGuid()) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UserId"/> class with the specified GUID.
    /// </summary>
    /// <param name="id">The GUID value of the user ID.</param>
 
    [JsonConstructor]
    public UserId(Guid id) : base(id)
    {
        Value = id;
    }

    /// <summary>
    /// Gets the GUID value of the user ID.
    /// </summary>
    [JsonPropertyName("id")]
    public override Guid Value { get; }
    

    /// <summary>
    /// Implicit conversion from <see cref="UserId"/> to <see cref="Guid"/>.
    /// </summary>
    /// <param name="id">The user ID.</param>
    public static implicit operator Guid(UserId id) => id?.Value ?? Guid.Empty;

    /// <summary>
    /// Implicit conversion from <see cref="Guid"/> to <see cref="UserId"/>.
    /// </summary>
    /// <param name="id">The GUID value.</param>
    public static implicit operator UserId(Guid id) => new(id);

    /// <summary>
    /// Determines whether the user ID is empty.
    /// </summary>
    public bool IsEmpty => Value == Guid.Empty;

    /// <inheritdoc />
    public bool Equals(UserId? other)
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
        return obj is UserId other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
    
    public static UserId New => new UserId(Guid.NewGuid());
    
    public static UserId Empty => new UserId(Guid.Empty);

    /// <summary>
    /// Tries to parse a string representation of a GUID to a <see cref="UserId"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="result">The parsed user ID.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public static bool TryParse(ReadOnlySpan<char> input, out UserId? result)
    {
        if (Guid.TryParse(input, out Guid result2))
        {
            result = new UserId(result2);
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Addition operator for <see cref="UserId"/> objects.
    /// </summary>
    /// <param name="a">The first user ID.</param>
    /// <param name="b">The second user ID.</param>
    /// <returns>A new user ID with the combined GUID value.</returns>
    public static UserId operator +(UserId a, UserId b) => new UserId(CombineGuids(a.Value, b.Value));

    /// <summary>
    /// Equality operator for <see cref="UserId"/> objects.
    /// </summary>
    /// <param name="a">The first user ID.</param>
    /// <param name="b">The second user ID.</param>
    /// <returns>True if the values are equal; otherwise, false.</returns>
    public static bool operator ==(UserId a, UserId b) => a?.Equals(b) ?? false;

    /// <summary>
    /// Inequality operator for <see cref="UserId"/> objects.
    /// </summary>
    /// <param name="a">The first user ID.</param>
    /// <param name="b">The second user ID.</param>
    /// <returns>True if the values are not equal; otherwise, false.</returns>
    public static bool operator !=(UserId a, UserId b) => !(a == b);

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
