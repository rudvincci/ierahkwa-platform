namespace Mamey.Identity.Decentralized.Abstractions;

public interface IMetaMaskService
{
    Task<bool> VerifyEthSignatureAsync(string challenge, string signature, string ethAddress);
}