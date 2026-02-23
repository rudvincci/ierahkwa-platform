using System.Security.Cryptography.X509Certificates;

namespace Mamey.Security;

public interface ICertificateGenerator<in TPrivateKey, TResult>
     where TPrivateKey : class, IPrivateKey
     where TResult : class, ICertificateResult<TPrivateKey>
{
    void ExportToFile(X509Certificate2 certificate, string filePath);
    TResult Generate(int keyLength = 50, bool pkHasSpecialCharacters = false, string? subject = null);
    TResult GenerateFromPrivateKey(TPrivateKey privateKey, string? subject = null);
}
