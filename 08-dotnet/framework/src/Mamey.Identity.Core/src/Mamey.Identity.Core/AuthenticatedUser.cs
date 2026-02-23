using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Mamey.Identity.Core
{
    public class AuthenticatedUser
    {
        public AuthenticatedUser()
        {
        }

        public AuthenticatedUser(AuthenticatedUser authenticatedUser)
            : this(authenticatedUser.UserId, authenticatedUser.TenantId,
                  authenticatedUser.Name, authenticatedUser.Email, authenticatedUser.Role,
                  authenticatedUser.AccessToken, authenticatedUser.RefreshToken,
                  authenticatedUser.Expires, authenticatedUser.Status, authenticatedUser.Type,
                  authenticatedUser.Claims)
        { }

        public AuthenticatedUser(Guid userId, Guid? tenantId, string? name,
            string? email, string? role, string? accessToken, string? refreshToken,
            long expires, string? status, string? type, IDictionary<string,
                string>? claims)
        {
            UserId = userId;
            Name = name;
            Email = email;
            Role = role;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Expires = expires;
            Status = status;
            Type = type;
            Claims = claims;
            TenantId = tenantId;
        }

        public Guid UserId { get; set; }
        public Guid? TenantId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public long Expires { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public IDictionary<string, string>? Claims { get; set; }

        [JsonIgnore]
        public bool Expired => DateTime.UtcNow > DateTime.UnixEpoch.AddSeconds(Expires);

        public event Action<AuthenticatedUser>? UserChanged;

        [JsonIgnore]
        public ClaimsPrincipal Principal
            => !HasClaims ? new ClaimsPrincipal(new ClaimsIdentity()) :
            new ClaimsPrincipal(ToClaimsIdentity());

        [JsonIgnore]
        public bool HasClaims => Claims is not null && Claims.Any() && ToClaimsEnumerable().Any();

        public IEnumerable<Claim> ToClaimsEnumerable()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
                new Claim(ClaimTypes.Expiration, Expires.ToString()),
                new Claim(ClaimTypes.Name, Name ?? string.Empty),
                new Claim(ClaimTypes.Role, Role ?? string.Empty),
                new Claim(ClaimTypes.Email, Email ?? string.Empty)
            };

            if (Claims != null && Claims.Any())
            {
                foreach (var claimSet in Claims)
                {
                    claims.Add(new Claim(claimSet.Key, claimSet.Value));
                }
            }

            return claims;
        }

        public ClaimsIdentity ToClaimsIdentity()
            => new ClaimsIdentity(ToClaimsEnumerable(), "mamey");
    }
}
