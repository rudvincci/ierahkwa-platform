using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mamey.Types;

namespace Mamey.Security;

public class CertificateGenerator<TPrivateKey> : ICertificateGenerator<TPrivateKey, ICertificateResult<TPrivateKey>>
    where TPrivateKey : class, IPrivateKey, new()
{
    private readonly AppOptions _appOptions;
    private readonly ICertificateProvider<TPrivateKey> _certificateProvider;

    public CertificateGenerator(AppOptions appOptions,
        ICertificateProvider<TPrivateKey> certificateProvider)
    {
        _appOptions = appOptions;
        _certificateProvider = certificateProvider;
    }

    public void ExportToFile(X509Certificate2 certificate, string filePath)
    {
        if (certificate == null)
            throw new ArgumentNullException(nameof(certificate));
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
        
        // Export certificate to a .crt file
        var certData = certificate.Export(X509ContentType.Cert);
        File.WriteAllBytes(filePath, certData);
    }

    public virtual ICertificateResult<TPrivateKey> Generate(int keyLength = 50, bool pkHasSpecialCharacters = false, string? subject = null)
    {
        var pk = _certificateProvider.GeneratePrivateKey(keyLength, pkHasSpecialCharacters);
        // Generate self-signed certificate using user and org private keys
        return GenerateFromPrivateKey(pk.PrivateKey, subject);
    }

    public virtual ICertificateResult<TPrivateKey> GenerateFromPrivateKey(TPrivateKey privateKey, string? subject = null)
    {
        if (privateKey == null)
            throw new ArgumentNullException(nameof(privateKey));
        
        if (_appOptions.OrganizationId is null || _appOptions.OrganizationId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(_appOptions.OrganizationId), "OrganizationId not set to an instance of a object.");
        }

        subject ??= $"CN=Certificate, O={_appOptions.OrganizationId}";

        // Generate self-signed certificate using user and org private keys
        var keyPair = RSA.Create(2048);
        var certificateRequest = new CertificateRequest(subject, keyPair, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        var certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));

        // Return the certificate and UserCertificateResult
        // Cast to the correct type - TPrivateKey should be PrivateKey in practice
        return new CertificateResult<TPrivateKey>(privateKey, certificate);
    }
}

//public class CertificateGenerator1<TPrivateKey>
//    : ICertificateGenerator<TPrivateKey, ICertificateGenerator<TPrivateKey, ICertificateResult<TPrivateKey>>>
//        where TPrivateKey : class, IPrivateKey, new()
//{
//    private readonly ILogger<CertificateGenerator<TPrivateKey>> _logger;
//    private readonly IRng _rng;
//    private readonly IEncryptor _encryptor;
//    private readonly ISigner _signer;
    
//    private readonly ICertificatesService _certificatesService;
    
//    private readonly JwtOptions _jwtOptions;
//    private readonly SecurityOptions _securityOptions;
//    private readonly VaultOptions _vaultOptions;
//    private readonly IPrivateKeyGenerator<TPrivateKey, IPrivateKeyResult<TPrivateKey>> _pkGenerator;

//    public CertificateGenerator1(ILogger<CertificateGenerator<TPrivateKey>> logger, IRng rng,
//        IEncryptor encryptor, ISigner signer, ICertificatesService certificatesService,
//        AppOptions appOptions, JwtOptions jwtOptions, SecurityOptions securityOptions,
//        VaultOptions vaultOptions, IPrivateKeyGenerator<TPrivateKey, IPrivateKeyResult<TPrivateKey>> pkGenerator,
//            ISecurityProvider securityProvider)
//    {
//        _logger = logger;
//        _rng = rng;
//        _encryptor = encryptor;
//        _signer = signer;
//        _certificatesService = certificatesService;
//        _appOptions = appOptions;
//        _jwtOptions = jwtOptions;
//        _securityOptions = securityOptions;
//        _vaultOptions = vaultOptions;
//        _pkGenerator = pkGenerator;
//        _securityProvider = securityProvider;
//    }
   

//    //public IPrivateKeyResult<TPrivateKey> GeneratePrivateKey(int length = 50, bool pkHasSpecialCharacters = false)
//    //{
//    //    var userPrivateKey = _rng.Generate(length, !pkHasSpecialCharacters);
//    //    var encryptedPrivateKey = _encryptor.Encrypt(userPrivateKey, _securityOptions.EncryptionKey);

//    //    string? pkSignature = null;
//    //    if (_vaultOptions.Enabled && _vaultOptions.Pki.Enabled)
//    //    {
//    //        var certificate = new X509Certificate2(_jwtOptions.Certificate.Location,
//    //            _jwtOptions.Certificate.Password);
//    //        pkSignature = _signer.Sign(encryptedPrivateKey, certificate);
//    //    }
//    //    var pk = new TPrivateKey();
//    //    pk.BindKey(pk.PrivateKeySignature);
//    //    pk.BindSignature(pk.EncryptedPrivateKeyHash);

//    //    return new PrivateKeyResult<TPrivateKey>(pk, userPrivateKey);
//    //}
//    //public bool VerifyPrivateKeySignature(TPrivateKey privateKey)
//    //{
//    //    if (string.IsNullOrEmpty(privateKey.PrivateKeySignature))
//    //    {
//    //        throw new MameyException("Private Key is not signed");
//    //    }
//    //    if (!_vaultOptions.Enabled || !_vaultOptions.Pki.Enabled)
//    //    {
//    //        throw new MameyException("Vault is disabled");
//    //    }
//    //    var publicKey = new X509Certificate2(_jwtOptions.Certificate.Location);
//    //    return _signer.Verify(privateKey.EncryptedPrivateKeyHash, publicKey, privateKey.PrivateKeySignature);
//    //}


//}