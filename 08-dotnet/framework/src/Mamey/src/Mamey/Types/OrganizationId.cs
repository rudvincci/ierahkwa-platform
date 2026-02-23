namespace Mamey.Types;

[Serializable]
public class OrganizationId : AggregateId<Guid>, IEquatable<OrganizationId>
{
    public OrganizationId() : this(Guid.NewGuid()) { }

    [JsonConstructor]
    public OrganizationId(Guid organizationId) : base(ValidateGuid(organizationId))
    {
        Value = organizationId;
    }

    [JsonPropertyName("organizationId")]
    public override Guid Value { get; }

    public static implicit operator Guid(OrganizationId id) => id.Value;

    public static implicit operator OrganizationId(Guid id) => new(id);

    /// <summary>
    /// Checks if the Guid is empty.
    /// </summary>
    public bool IsEmpty => Value == Guid.Empty;

    /// <summary>
    /// Validates that the Guid is not empty.
    /// </summary>
    /// <param name="guid">The Guid to validate.</param>
    /// <returns>The validated Guid.</returns>
    /// <exception cref="ArgumentException">Thrown when the Guid is empty.</exception>
    private static Guid ValidateGuid(Guid guid)
    {
        if (guid == Guid.Empty)
            throw new ArgumentException("OrganizationId cannot be an empty Guid.", nameof(guid));
        return guid;
    }

    /// <summary>
    /// Checks equality with another OrganizationId instance.
    /// </summary>
    /// <param name="other">The other OrganizationId instance.</param>
    /// <returns>True if equal, false otherwise.</returns>
    public bool Equals(OrganizationId? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    /// <summary>
    /// Checks equality with another object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if equal, false otherwise.</returns>
    public override bool Equals(object? obj)
    {
        return obj is OrganizationId other && Equals(other);
    }

    /// <summary>
    /// Gets the hash code for the instance.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Converts the OrganizationId to a string.
    /// </summary>
    /// <returns>The string representation of the OrganizationId.</returns>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Attempts to parse a string into an OrganizationId.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="result">The parsed OrganizationId instance.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool TryParse(ReadOnlySpan<char> input, out OrganizationId result)
    {
        if (Guid.TryParse(input, out var parsedGuid) && parsedGuid != Guid.Empty)
        {
            result = new OrganizationId(parsedGuid);
            return true;
        }

        result = default!;
        return false;
    }

    /// <summary>
    /// Parses a string into an OrganizationId or throws an exception if invalid.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The parsed OrganizationId.</returns>
    /// <exception cref="FormatException">Thrown if the input is not a valid Guid.</exception>
    public static OrganizationId Parse(string input)
    {
        if (Guid.TryParse(input, out var parsedGuid) && parsedGuid != Guid.Empty)
        {
            return new OrganizationId(parsedGuid);
        }

        throw new FormatException("Input is not a valid non-empty Guid.");
    }
}
