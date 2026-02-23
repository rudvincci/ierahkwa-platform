namespace Mamey.Auth.Abstractions;

/// <summary>
/// Represents the result of a token validation operation.
/// </summary>
public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public string? UserId { get; set; }
    public IEnumerable<KeyValuePair<string, string>> Claims { get; set; } = new List<KeyValuePair<string, string>>();
}