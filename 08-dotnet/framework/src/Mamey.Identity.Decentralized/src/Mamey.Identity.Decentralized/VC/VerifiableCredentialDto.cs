namespace Mamey.Identity.Decentralized.VC;

/// <summary>
/// Verifiable Credential model for API responses.
/// </summary>
public class VerifiableCredentialDto
{
    public string Id { get; set; }
    public string Context { get; set; }
    public string Type { get; set; }
    public string Issuer { get; set; }
    public string Subject { get; set; }
    public DateTimeOffset IssuanceDate { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public Dictionary<string, object> Claims { get; set; }
    public string Proof { get; set; } // JWT or Linked Data Proof JSON
    public string CredentialStatus { get; set; }
}