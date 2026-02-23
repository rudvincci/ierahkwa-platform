namespace Mamey.MicroMonolith.Abstractions.Auth;

public class JsonWebToken
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public long? Expiry { get; set; }
    public Guid UserId { get; set; }
    public string? Role { get; set; }
    public string Email { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public IDictionary<string, IEnumerable<string>> Claims { get; set; }
}