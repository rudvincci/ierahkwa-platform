using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AppBuilder.Infrastructure.Services;

/// <summary>Auth - JWT, email/password, social. IERAHKWA Appy. GDPR: export/delete in UsersController.</summary>
public class AuthService : IAuthService
{
    private static readonly List<User> _users = new();
    private static readonly object _lock = new();
    private readonly JwtOptions _jwt;
    private readonly ILogger<AuthService> _log;

    public AuthService(IOptions<JwtOptions> jwt, ILogger<AuthService> log)
    {
        _jwt = jwt.Value;
        _log = log;
    }

    public (User User, string Token) Register(string email, string password, string? name = null)
    {
        lock (_lock)
        {
            if (_users.Any(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Email already registered");

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Name = name ?? email.Split('@')[0],
                PlanTier = PlanTier.Free,
                BuildCredits = 5,
                CreatedAt = DateTime.UtcNow
            };
            _users.Add(user);
            _log.LogInformation("IERAHKWA Appy: User registered {Email}", email);
            var token = GenerateToken(user);
            return (user, token);
        }
    }

    public (User? User, string? Token) Login(string email, string password)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
            if (user == null || user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return (null, null);
            user.LastLoginAt = DateTime.UtcNow;
            var token = GenerateToken(user);
            return (user, token);
        }
    }

    public User? GetUserById(string id)
    {
        lock (_lock) return _users.FirstOrDefault(u => u.Id == id);
    }

    public User? GetUserByEmail(string email)
    {
        lock (_lock) return _users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    public void UpdateLastLogin(string userId)
    {
        lock (_lock)
        {
            var u = _users.FirstOrDefault(x => x.Id == userId);
            if (u != null) u.LastLoginAt = DateTime.UtcNow;
        }
    }

    public (User? User, string? Token) SocialLogin(string provider, string providerId, string email, string? name, string? avatarUrl)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.SocialProvider == provider && u.SocialId == providerId);
            if (user == null)
            {
                user = _users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
                if (user != null)
                {
                    user.SocialProvider = provider;
                    user.SocialId = providerId;
                    user.AvatarUrl = avatarUrl ?? user.AvatarUrl;
                }
                else
                {
                    user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        Name = name ?? email.Split('@')[0],
                        SocialProvider = provider,
                        SocialId = providerId,
                        AvatarUrl = avatarUrl,
                        PlanTier = PlanTier.Free,
                        BuildCredits = 5,
                        CreatedAt = DateTime.UtcNow
                    };
                    _users.Add(user);
                }
            }
            user.LastLoginAt = DateTime.UtcNow;
            var token = GenerateToken(user);
            return (user, token);
        }
    }

    public bool TryConsumeBuildCredit(string userId)
    {
        lock (_lock)
        {
            var u = _users.FirstOrDefault(x => x.Id == userId);
            if (u == null || u.BuildCredits <= 0) return false;
            u.BuildCredits--;
            return true;
        }
    }

    public void GrantBuildCredits(string userId, int count)
    {
        lock (_lock)
        {
            var u = _users.FirstOrDefault(x => x.Id == userId);
            if (u != null) u.BuildCredits += count;
        }
    }

    public bool DeleteUser(string userId)
    {
        lock (_lock)
        {
            var u = _users.FirstOrDefault(x => x.Id == userId);
            if (u == null) return false;
            _users.Remove(u);
            _log.LogInformation("IERAHKWA Appy: User deleted (GDPR) {UserId}", userId);
            return true;
        }
    }

    public int GetUserCount() { lock (_lock) return _users.Count; }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name ?? user.Email)
        };
        var token = new JwtSecurityToken(_jwt.Issuer, _jwt.Audience, claims, DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes), creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
