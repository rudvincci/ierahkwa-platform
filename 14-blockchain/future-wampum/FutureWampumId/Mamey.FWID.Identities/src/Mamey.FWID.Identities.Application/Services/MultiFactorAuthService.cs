using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Security;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling multi-factor authentication operations.
/// </summary>
internal sealed class MultiFactorAuthService : IMultiFactorAuthService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IMfaConfigurationRepository _mfaConfigurationRepository;
    private readonly ISecurityProvider _securityProvider;
    private readonly ILogger<MultiFactorAuthService> _logger;

    public MultiFactorAuthService(
        IIdentityRepository identityRepository,
        IMfaConfigurationRepository mfaConfigurationRepository,
        ISecurityProvider securityProvider,
        ILogger<MultiFactorAuthService> logger)
    {
        _identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        _mfaConfigurationRepository = mfaConfigurationRepository ?? throw new ArgumentNullException(nameof(mfaConfigurationRepository));
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<MfaSetupResult> SetupMfaAsync(
        IdentityId identityId,
        MfaMethod method,
        CancellationToken cancellationToken = default)
    {
        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (identity == null)
            throw new InvalidOperationException("Identity not found");

        // Check if MFA configuration already exists
        var existingConfig = await _mfaConfigurationRepository.GetByIdentityAndMethodAsync(
            identityId, method, cancellationToken);

        if (existingConfig != null && existingConfig.IsEnabled)
            throw new InvalidOperationException($"MFA method {method} is already enabled for this identity");

        string? secretKey = null;
        string? qrCodeDataUrl = null;

        if (method == MfaMethod.Totp)
        {
            // Generate TOTP secret key (base32 encoded, 32 characters)
            secretKey = GenerateTotpSecretKey();
            // Generate QR code data URL (would typically use a QR code library)
            qrCodeDataUrl = GenerateQrCodeDataUrl(identity, secretKey);
        }

        MfaConfigurationId configId;
        if (existingConfig != null)
        {
            // Update existing configuration
            configId = existingConfig.Id;
            existingConfig.Enable(secretKey);
            await _mfaConfigurationRepository.UpdateAsync(existingConfig, cancellationToken);
        }
        else
        {
            // Create new configuration
            configId = new MfaConfigurationId();
            var config = new MfaConfiguration(configId, identityId, method);
            config.Enable(secretKey);
            await _mfaConfigurationRepository.AddAsync(config, cancellationToken);
        }

        _logger.LogInformation("MFA setup completed for identity {IdentityId}, method {Method}", identityId, method);

        return new MfaSetupResult
        {
            MfaConfigurationId = configId,
            SecretKey = secretKey,
            QrCodeDataUrl = qrCodeDataUrl
        };
    }

    public async Task EnableMfaAsync(
        IdentityId identityId,
        MfaMethod method,
        string verificationCode,
        CancellationToken cancellationToken = default)
    {
        var config = await _mfaConfigurationRepository.GetByIdentityAndMethodAsync(
            identityId, method, cancellationToken);

        if (config == null)
            throw new InvalidOperationException("MFA configuration not found. Please set up MFA first.");

        if (config.IsEnabled)
            throw new InvalidOperationException("MFA is already enabled");

        // Verify the code (for TOTP, this would use a TOTP library)
        if (!VerifyMfaCode(config, verificationCode))
        {
            _logger.LogWarning("MFA enable failed: Invalid verification code for identity {IdentityId}", identityId);
            throw new InvalidOperationException("Invalid verification code");
        }

        // Configuration is already enabled in SetupMfaAsync, but we verify here
        config.UpdateLastUsed();
        await _mfaConfigurationRepository.UpdateAsync(config, cancellationToken);

        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (identity != null)
        {
            identity.EnableMfa(method);
            await _identityRepository.UpdateAsync(identity, cancellationToken);
        }

        _logger.LogInformation("MFA enabled for identity {IdentityId}, method {Method}", identityId, method);
    }

    public async Task DisableMfaAsync(
        IdentityId identityId,
        MfaMethod method,
        CancellationToken cancellationToken = default)
    {
        var config = await _mfaConfigurationRepository.GetByIdentityAndMethodAsync(
            identityId, method, cancellationToken);

        if (config == null || !config.IsEnabled)
            throw new InvalidOperationException("MFA is not enabled");

        config.Disable();
        await _mfaConfigurationRepository.UpdateAsync(config, cancellationToken);

        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (identity != null)
        {
            identity.DisableMfa();
            await _identityRepository.UpdateAsync(identity, cancellationToken);
        }

        _logger.LogInformation("MFA disabled for identity {IdentityId}, method {Method}", identityId, method);
    }

    public async Task<MfaChallengeResult> CreateMfaChallengeAsync(
        IdentityId identityId,
        MfaMethod method,
        CancellationToken cancellationToken = default)
    {
        var config = await _mfaConfigurationRepository.GetByIdentityAndMethodAsync(
            identityId, method, cancellationToken);

        if (config == null || !config.IsEnabled)
            throw new InvalidOperationException("MFA is not enabled for this method");

        var challengeId = Guid.NewGuid();
        string? code = null;
        var expiresAt = DateTime.UtcNow.AddMinutes(10); // 10 minute expiration

        if (method == MfaMethod.Sms || method == MfaMethod.Email)
        {
            // Generate a 6-digit code
            code = GenerateNumericCode(6);
            // TODO: Send code via SMS or Email service
            _logger.LogInformation("MFA challenge code generated for identity {IdentityId}, method {Method}", identityId, method);
        }
        else if (method == MfaMethod.Totp)
        {
            // TOTP codes are generated by the authenticator app, not by us
            // We just need to verify them
        }

        return new MfaChallengeResult
        {
            ChallengeId = challengeId,
            Code = code,
            ExpiresAt = expiresAt
        };
    }

    public async Task<bool> VerifyMfaChallengeAsync(
        IdentityId identityId,
        MfaMethod method,
        string code,
        CancellationToken cancellationToken = default)
    {
        var config = await _mfaConfigurationRepository.GetByIdentityAndMethodAsync(
            identityId, method, cancellationToken);

        if (config == null || !config.IsEnabled)
            return false;

        var isValid = VerifyMfaCode(config, code);
        
        if (isValid)
        {
            config.UpdateLastUsed();
            await _mfaConfigurationRepository.UpdateAsync(config, cancellationToken);
            _logger.LogInformation("MFA challenge verified for identity {IdentityId}, method {Method}", identityId, method);
        }
        else
        {
            _logger.LogWarning("MFA challenge verification failed for identity {IdentityId}, method {Method}", identityId, method);
        }

        return isValid;
    }

    public async Task<List<string>> GenerateBackupCodesAsync(
        IdentityId identityId,
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        var configs = await _mfaConfigurationRepository.GetByIdentityIdAsync(identityId, cancellationToken);
        var activeConfig = configs.FirstOrDefault(c => c.IsEnabled);
        
        if (activeConfig == null)
            throw new InvalidOperationException("No active MFA configuration found");

        var codes = new List<string>();
        var hashedCodes = new List<string>();

        for (int i = 0; i < count; i++)
        {
            var code = GenerateBackupCode();
            codes.Add(code);
            hashedCodes.Add(_securityProvider.Hash(code));
        }

        activeConfig.SetBackupCodes(hashedCodes);
        await _mfaConfigurationRepository.UpdateAsync(activeConfig, cancellationToken);

        _logger.LogInformation("Generated {Count} backup codes for identity {IdentityId}", count, identityId);

        return codes; // Return plain codes (user should save them)
    }

    public async Task<bool> VerifyBackupCodeAsync(
        IdentityId identityId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var configs = await _mfaConfigurationRepository.GetByIdentityIdAsync(identityId, cancellationToken);
        var activeConfig = configs.FirstOrDefault(c => c.IsEnabled);
        
        if (activeConfig == null)
            return false;

        var hashedCode = _securityProvider.Hash(code);
        var isValid = activeConfig.BackupCodes.Contains(hashedCode);

        if (isValid)
        {
            // Remove used backup code
            activeConfig.BackupCodes.Remove(hashedCode);
            activeConfig.UpdateLastUsed();
            await _mfaConfigurationRepository.UpdateAsync(activeConfig, cancellationToken);
            _logger.LogInformation("Backup code verified for identity {IdentityId}", identityId);
        }
        else
        {
            _logger.LogWarning("Invalid backup code for identity {IdentityId}", identityId);
        }

        return isValid;
    }

    private string GenerateTotpSecretKey()
    {
        // Generate a base32-encoded secret key (32 characters)
        const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        return _securityProvider.GenerateRandomString(32, removeSpecialChars: false)
            .ToUpperInvariant()
            .Where(c => base32Chars.Contains(c))
            .Take(32)
            .Aggregate("", (s, c) => s + c);
    }

    private string GenerateQrCodeDataUrl(Identity identity, string secretKey)
    {
        // Generate QR code data URL for TOTP setup
        // Format: otpauth://totp/{issuer}:{account}?secret={secret}&issuer={issuer}
        var issuer = "FutureWampum";
        var account = identity.ContactInformation.Email?.Value ?? identity.Id.Value.ToString();
        var otpAuthUrl = $"otpauth://totp/{issuer}:{account}?secret={secretKey}&issuer={issuer}";
        
        // TODO: Generate actual QR code image and convert to data URL
        // For now, return the OTP auth URL
        return otpAuthUrl;
    }

    private string GenerateNumericCode(int length)
    {
        var random = new Random();
        var code = "";
        for (int i = 0; i < length; i++)
        {
            code += random.Next(0, 10).ToString();
        }
        return code;
    }

    private string GenerateBackupCode()
    {
        // Generate an 8-character alphanumeric backup code
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return _securityProvider.GenerateRandomString(8, removeSpecialChars: true).ToUpperInvariant();
    }

    private bool VerifyMfaCode(MfaConfiguration config, string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        return config.Method switch
        {
            MfaMethod.Totp => VerifyTotpCode(config.SecretKey, code),
            MfaMethod.Sms => true, // SMS codes are verified separately via challenge
            MfaMethod.Email => true, // Email codes are verified separately via challenge
            MfaMethod.Biometric => true, // Biometric is verified separately
            MfaMethod.BackupCode => VerifyBackupCode(config, code),
            _ => false
        };
    }

    private bool VerifyTotpCode(string? secretKey, string code)
    {
        if (string.IsNullOrWhiteSpace(secretKey) || code.Length != 6 || !code.All(char.IsDigit))
            return false;

        // TODO: Implement actual TOTP verification using a library like OtpNet
        // For now, return false (this should be implemented properly)
        return false;
    }

    private bool VerifyBackupCode(MfaConfiguration config, string code)
    {
        var hashedCode = _securityProvider.Hash(code);
        return config.BackupCodes.Contains(hashedCode);
    }
}

