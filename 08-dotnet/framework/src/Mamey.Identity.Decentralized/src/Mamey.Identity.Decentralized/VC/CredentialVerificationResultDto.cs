namespace Mamey.Identity.Decentralized.VC;

public class CredentialVerificationResultDto
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public string CredentialId { get; set; }
    public string Issuer { get; set; }
    public string Subject { get; set; }
    public List<string> Warnings { get; set; }
    public List<string> Errors { get; set; }
}