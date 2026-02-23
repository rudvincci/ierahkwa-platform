using System.Security.Cryptography.X509Certificates;
using Mamey.Exceptions;
using Mamey.Secrets.Vault;

namespace Mamey.Security;

public class PrivateKeyService : IPrivateKeyService
{
    private readonly IRng _rng;
    private readonly IEncryptor _encryptor;
    private readonly ISigner _signer;
    // private readonly ICertificatesService _certificatesService;
    private readonly VaultOptions _vaultOptions;
    private readonly SecurityOptions _securityOptions;


    public PrivateKeyService(IRng rng, IEncryptor encryptor, ISigner signer,
        // ICertificatesService certificatesService,
        VaultOptions vaultOptions,
        SecurityOptions securityOptions)
    {
        _rng = rng;
        _encryptor = encryptor;
        _signer = signer;
        // _certificatesService = certificatesService;
        _vaultOptions = vaultOptions;
        _securityOptions = securityOptions;

    }

    public (PrivateKey, string) Generate(int length = 50)
    {
        var userPrivateKey = _rng.Generate(length, true);


        var encryptedPrivateKey = _encryptor.Encrypt(userPrivateKey, _securityOptions.EncryptionKey);

        string? pkSignature = null;
        if (!string.IsNullOrEmpty(_securityOptions.Certificate.Location) && !string.IsNullOrEmpty(_securityOptions.Certificate.Password))//_vaultOptions.Enabled && _vaultOptions.Pki.Enabled)
        {
            var certificate = new X509Certificate2(_securityOptions.Certificate.Location, _securityOptions.Certificate.Password);
            pkSignature = _signer.Sign(encryptedPrivateKey, certificate);
        }
        return (new(encryptedPrivateKey, pkSignature), userPrivateKey);
    }

    public bool ValidatePrivateKey(PrivateKey privateKey)
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
        var publicKey = new X509Certificate2("certs/localhost.crt");
        return _signer.Verify(privateKey.EncryptedPrivateKey, publicKey, privateKey.PrivateKeySignature);
    }
}