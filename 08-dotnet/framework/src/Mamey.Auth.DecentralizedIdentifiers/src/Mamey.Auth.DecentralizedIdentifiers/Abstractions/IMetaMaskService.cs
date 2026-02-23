namespace Mamey.Auth.DecentralizedIdentifiers.Abstractions;

public interface IMetaMaskService
{
    Task<bool> VerifyEthSignatureAsync(string challenge, string signature, string ethAddress);
}