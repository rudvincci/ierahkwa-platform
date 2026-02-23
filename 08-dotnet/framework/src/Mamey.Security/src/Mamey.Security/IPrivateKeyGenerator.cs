namespace Mamey.Security;

public interface IPrivateKeyGenerator<in TPrivateKey, TResult>
     where TPrivateKey : class, IPrivateKey
     where TResult : class, IPrivateKeyResult<TPrivateKey>
{
    TResult Generate(int length = 50, bool pkHasSpecialCharacters = false);
    bool VerifyPrivateKeySignature(TPrivateKey privateKey);
}

//Task<IPrivateKeyResult<TPrivateKey>> GenerateAsync<TPrivateKey>(TPrivateKey privateKey, CancellationToken cancellationToken = default);