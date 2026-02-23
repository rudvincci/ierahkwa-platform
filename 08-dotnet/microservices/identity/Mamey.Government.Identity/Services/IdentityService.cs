using Mamey.Government.Identity.Models;

namespace Mamey.Government.Identity.Services;

public interface IIdentityService
{
    Task<GovernmentIdentity> RegisterAsync(RegisterIdentityRequest request);
    Task<GovernmentIdentity?> GetByIdAsync(Guid id);
    Task<GovernmentIdentity?> GetByFwIdAsync(string futureWampumId);
    Task<GovernmentIdentity?> GetByEmailAsync(string email);
    Task<GovernmentIdentity?> GetByWalletAsync(string walletAddress);
    Task<bool> VerifyIdentityAsync(Guid id, VerificationRequest request);
    Task<bool> LinkWalletAsync(Guid id, string walletAddress, string publicKey);
    Task<bool> SetMembershipAsync(Guid id, MembershipTier tier);
    Task<string> GenerateFutureWampumIdAsync();
    Task<AuthToken> AuthenticateAsync(AuthRequest request);
    Task<bool> ValidateTokenAsync(string token);
}

public class IdentityService : IIdentityService
{
    private readonly Dictionary<Guid, GovernmentIdentity> _identities = new();
    private readonly Dictionary<string, Guid> _fwidIndex = new();
    private readonly Dictionary<string, Guid> _emailIndex = new();
    private readonly Dictionary<string, Guid> _walletIndex = new();
    private long _fwidCounter = 1000000;
    
    public async Task<GovernmentIdentity> RegisterAsync(RegisterIdentityRequest request)
    {
        // Generate FutureWampumID
        var fwid = await GenerateFutureWampumIdAsync();
        
        var identity = new GovernmentIdentity
        {
            FutureWampumId = fwid,
            CitizenId = $"CIT{DateTime.UtcNow:yyyyMMdd}{_fwidCounter:D6}",
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email,
            Phone = request.Phone,
            VerificationLevel = VerificationLevel.Email
        };
        
        // Generate referral code
        identity.ReferralCode = $"REF{fwid[4..]}";
        
        _identities[identity.Id] = identity;
        _fwidIndex[fwid] = identity.Id;
        _emailIndex[request.Email.ToLowerInvariant()] = identity.Id;
        
        return identity;
    }
    
    public Task<GovernmentIdentity?> GetByIdAsync(Guid id)
    {
        _identities.TryGetValue(id, out var identity);
        return Task.FromResult(identity);
    }
    
    public Task<GovernmentIdentity?> GetByFwIdAsync(string futureWampumId)
    {
        if (_fwidIndex.TryGetValue(futureWampumId, out var id))
        {
            _identities.TryGetValue(id, out var identity);
            return Task.FromResult(identity);
        }
        return Task.FromResult<GovernmentIdentity?>(null);
    }
    
    public Task<GovernmentIdentity?> GetByEmailAsync(string email)
    {
        if (_emailIndex.TryGetValue(email.ToLowerInvariant(), out var id))
        {
            _identities.TryGetValue(id, out var identity);
            return Task.FromResult(identity);
        }
        return Task.FromResult<GovernmentIdentity?>(null);
    }
    
    public Task<GovernmentIdentity?> GetByWalletAsync(string walletAddress)
    {
        if (_walletIndex.TryGetValue(walletAddress.ToLowerInvariant(), out var id))
        {
            _identities.TryGetValue(id, out var identity);
            return Task.FromResult(identity);
        }
        return Task.FromResult<GovernmentIdentity?>(null);
    }
    
    public Task<bool> VerifyIdentityAsync(Guid id, VerificationRequest request)
    {
        if (!_identities.TryGetValue(id, out var identity))
            return Task.FromResult(false);
        
        identity.SetVerificationLevel(request.Level, request.Method);
        
        if (request.BiometricHash != null)
            identity.BiometricHash = request.BiometricHash;
        
        if (request.KycProofHash != null)
        {
            identity.KycProofHash = request.KycProofHash;
            identity.KycStatus = KycStatus.Approved;
            identity.KycCompletedAt = DateTime.UtcNow;
        }
        
        return Task.FromResult(true);
    }
    
    public Task<bool> LinkWalletAsync(Guid id, string walletAddress, string publicKey)
    {
        if (!_identities.TryGetValue(id, out var identity))
            return Task.FromResult(false);
        
        identity.LinkWallet(walletAddress, publicKey);
        _walletIndex[walletAddress.ToLowerInvariant()] = id;
        
        return Task.FromResult(true);
    }
    
    public Task<bool> SetMembershipAsync(Guid id, MembershipTier tier)
    {
        if (!_identities.TryGetValue(id, out var identity))
            return Task.FromResult(false);
        
        identity.SetMembership(tier);
        return Task.FromResult(true);
    }
    
    public Task<string> GenerateFutureWampumIdAsync()
    {
        var id = Interlocked.Increment(ref _fwidCounter);
        var fwid = $"FWID{id:D10}";
        return Task.FromResult(fwid);
    }
    
    public Task<AuthToken> AuthenticateAsync(AuthRequest request)
    {
        // Simple token generation (in production use proper JWT)
        var token = new AuthToken
        {
            AccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            RefreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            TokenType = "Bearer"
        };
        
        return Task.FromResult(token);
    }
    
    public Task<bool> ValidateTokenAsync(string token)
    {
        // Token validation logic
        return Task.FromResult(!string.IsNullOrEmpty(token));
    }
}

// DTOs
public record RegisterIdentityRequest(
    string FirstName,
    string LastName,
    string? MiddleName,
    DateTime DateOfBirth,
    string Email,
    string? Phone
);

public record VerificationRequest(
    VerificationLevel Level,
    string Method,
    string? BiometricHash = null,
    string? KycProofHash = null
);

public record AuthRequest(
    string Email,
    string? Password,
    string? WalletSignature,
    string? BiometricToken
);

public record AuthToken
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public string TokenType { get; init; } = "Bearer";
}
