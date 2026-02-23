namespace Pupitre.Types.Identifiers;

/// <summary>
/// Strongly-typed identifier for Credential entities.
/// </summary>
public sealed record CredentialId
{
    public Guid Value { get; }

    public CredentialId() : this(Guid.NewGuid()) { }
    public CredentialId(Guid value) => Value = value;

    public static CredentialId New() => new(Guid.NewGuid());
    public static CredentialId Empty => new(Guid.Empty);

    public static implicit operator CredentialId(Guid value) => new(value);
    public static implicit operator Guid(CredentialId id) => id.Value;

    public override string ToString() => Value.ToString();
}
