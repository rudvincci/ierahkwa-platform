namespace Mamey.Identity.Decentralized.VC;

public interface ICredentialService
{
    Task<VerifiableCredentialDto> IssueCredentialAsync(CredentialIssueRequest request);
    Task<CredentialVerificationResultDto> VerifyCredentialAsync(CredentialVerifyRequest request);
    Task RevokeCredentialAsync(string credentialId);
}