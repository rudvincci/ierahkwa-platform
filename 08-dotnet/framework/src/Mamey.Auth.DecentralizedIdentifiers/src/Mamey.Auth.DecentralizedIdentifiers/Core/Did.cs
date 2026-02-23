using System.Text.RegularExpressions;

namespace Mamey.Auth.DecentralizedIdentifiers.Core;

/// <summary>
/// Represents a W3C Decentralized Identifier (DID) according to the DID Core specification.
/// </summary>
public class Did
{
    /// <summary>
    /// Gets the canonical DID string (e.g., "did:example:123456").
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The method-specific identifier (portion after the method name).
    /// </summary>
    public string MethodSpecificId { get; }

    /// <summary>
    /// The DID method (e.g., "key", "web", "ion").
    /// </summary>
    public string Method { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Did"/> class and validates the input.
    /// </summary>
    /// <param name="did">A W3C DID string.</param>
    public Did(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
            throw new ArgumentNullException(nameof(did), "DID cannot be null or empty.");

        // DID Syntax: did:<method>:<method-specific-id>
        var regex = new Regex(@"^did:([a-z0-9]+):([a-zA-Z0-9.\-_:]+)$", RegexOptions.Compiled);
        var match = regex.Match(did);
        if (!match.Success)
            throw new FormatException($"'{did}' is not a valid DID.");

        Value = did;
        Method = match.Groups[1].Value;
        MethodSpecificId = match.Groups[2].Value;
    }

    /// <summary>
    /// Returns the canonical string representation of the DID.
    /// </summary>
    public override string ToString() => Value;
}