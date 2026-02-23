namespace Mamey.Security;

public interface IPrivateKeyService
{
    (PrivateKey, string) Generate(int length = 50);
    bool ValidatePrivateKey(PrivateKey privateKey);
}