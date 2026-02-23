namespace Mamey.Auth.DecentralizedIdentifiers.Abstractions;

public interface IFido2Service
{
    Task<bool> VerifyFido2AssertionAsync(string challenge, string response);
}