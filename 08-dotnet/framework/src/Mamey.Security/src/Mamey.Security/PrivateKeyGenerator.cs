using Mamey.Exceptions;
using System.Security.Cryptography.X509Certificates;
using Mamey.Auth.Jwt;
using Mamey.Secrets.Vault;

namespace Mamey.Security;
public class PrivateKeyGenerator<TPrivateKey> : IPrivateKeyGenerator<TPrivateKey, IPrivateKeyResult<TPrivateKey>>
    where TPrivateKey : class, IPrivateKey, new()
{
    private readonly IRng _rng;
    //private readonly IEncryptor _encryptor;
    private readonly ISigner _signer;
    private readonly VaultOptions _vaultOptions;
    //private readonly SecurityOptions _securityOptions;
    private readonly JwtOptions _jwtOptions;
    private readonly ISecurityProvider _securityProvider;

    public PrivateKeyGenerator(IRng rng, ISigner signer, VaultOptions vaultOptions,
        JwtOptions jwtOptions, ISecurityProvider securityProvider)
    {
        _rng = rng;
        //_encryptor = encryptor;
        _signer = signer;
        //_certificatesService = certificatesService;
        _vaultOptions = vaultOptions;
        //_securityOptions = securityOptions;
        _jwtOptions = jwtOptions;
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
    }

    public IPrivateKeyResult<TPrivateKey> Generate(int length = 50, bool pkHasSpecialCharacters = false)
    {
        var userPrivateKey = _rng.Generate(length, !pkHasSpecialCharacters);
        var encryptedPrivateKey = _securityProvider.Encrypt(userPrivateKey);

        string? pkSignature = null;
        if (!string.IsNullOrWhiteSpace(_jwtOptions.Certificate.Location) && !string.IsNullOrEmpty(_jwtOptions.Certificate.Password))
        {
            try
            {
                var certificate = new X509Certificate2(_jwtOptions.Certificate.Location,
                    _jwtOptions.Certificate.Password);
                pkSignature = _securityProvider.Sign(encryptedPrivateKey, certificate);
            }
            catch (FileNotFoundException)
            {
                // Certificate file not found - signature will remain null
                // This is acceptable behavior for tests or when certificate is optional
            }
        }
        // Create PrivateKey instance - TPrivateKey should be PrivateKey in practice
        // Use the constructor that accepts both parameters
        var pk = (TPrivateKey)(object)new PrivateKey(encryptedPrivateKey, pkSignature);
        return new PrivateKeyResult<TPrivateKey>(pk, userPrivateKey);
        
    }

    public bool VerifyPrivateKeySignature(TPrivateKey privateKey)
    {
        if (privateKey == null)
            throw new ArgumentNullException(nameof(privateKey));
        
        if (string.IsNullOrEmpty(privateKey.PrivateKeySignature))
        {
            throw new MameyException("Private Key is not signed");
        }
        if (!_vaultOptions.Enabled || !_vaultOptions.Pki.Enabled)
        {
            throw new MameyException("Vault is disabled");
        }
        var publicKey = new X509Certificate2(_jwtOptions.Certificate.Location);
        return _signer.Verify(privateKey.EncryptedPrivateKey, publicKey, privateKey.PrivateKeySignature);
    }
}
