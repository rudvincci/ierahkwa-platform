namespace Mamey.Identity.Decentralized.Abstractions;

public interface IVerificationMethod
{
    string Id { get; }
    string Type { get; }
    string Controller { get; }
    byte[] GetPublicKeyBytes();
    // (possibly other fields)
}