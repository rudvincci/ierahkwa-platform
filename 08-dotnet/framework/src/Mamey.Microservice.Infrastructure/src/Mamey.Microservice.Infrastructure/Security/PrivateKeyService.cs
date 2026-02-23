//using System.Security.Cryptography.X509Certificates;
//using Mamey.Auth;
//using Mamey.Secrets.Vault;
//using Mamey.Security;
//using Mamey.Microservice.Abstractions.Cryptology;


//namespace Mamey.Microservice.Infrastructure.Security;

//internal class PrivateKeyService : IPrivateKeyService
//{
//    private readonly IRng _rng;
//    private readonly IEncryptor _encryptor;
//    private readonly ISigner _signer;
//    private readonly ICertificatesService _certificatesService;
//    private readonly VaultOptions _vaultOptions;
//    private readonly SecurityOptions _securityOptions;
//    private readonly JwtOptions _jwtOptions;

//    public PrivateKeyService(IRng rng, IEncryptor encryptor, ISigner signer, ICertificatesService certificatesService, VaultOptions vaultOptions, SecurityOptions securityOptions, JwtOptions jwtOptions)
//    {
//        _rng = rng;
//        _encryptor = encryptor;
//        _signer = signer;
//        _certificatesService = certificatesService;
//        _vaultOptions = vaultOptions;
//        _securityOptions = securityOptions;
//        _jwtOptions = jwtOptions;
//    }

//    public (PrivateKey, string) Generate(int length = 50)
//    {
//        var userPrivateKey = _rng.Generate(length,true);
        

//        var encryptedPrivateKey = _encryptor.Encrypt(userPrivateKey, _securityOptions.EncryptionKey);

//        string? pkSignature = null;
//        if (_vaultOptions.Enabled && _vaultOptions.Pki.Enabled)
//        {
//            var certificate = new X509Certificate2(_jwtOptions.Certificate.Location, _jwtOptions.Certificate.Password);
//            pkSignature = _signer.Sign(encryptedPrivateKey, certificate);
//        }
//        return (new(encryptedPrivateKey, pkSignature), userPrivateKey);
//    }

//    public bool ValidatePrivateKey(PrivateKey privateKey)
//    {
//        if(string.IsNullOrEmpty(privateKey.Signature))
//        {
//            throw new MameyException("Private Key is not signed");
//        }
//        if (!_vaultOptions.Enabled || !_vaultOptions.Pki.Enabled)
//        {
//            throw new MameyException("Vault is disabled");
//        }
//        var publicKey = new X509Certificate2("certs/mamey-dev.crt");
//        return _signer.Verify(privateKey.EncryptedPrivateKeyHash, publicKey, privateKey.Signature);
//    }
//}

