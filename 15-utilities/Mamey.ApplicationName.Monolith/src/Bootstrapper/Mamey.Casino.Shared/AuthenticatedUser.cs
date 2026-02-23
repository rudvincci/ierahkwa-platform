using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Mamey.Casino.Shared;

public class AuthenticatedUser
    {
        public AuthenticatedUser()
        {
        }

        public AuthenticatedUser(AuthenticatedUser authenticatedUser)
            : this(authenticatedUser.UserId, authenticatedUser.OrganizationId,
                  authenticatedUser.Name, authenticatedUser.Email, authenticatedUser.Role,
                  authenticatedUser.AccessToken, authenticatedUser.RefreshToken,
                  authenticatedUser.Expires, authenticatedUser.Status, authenticatedUser.Type,
                  authenticatedUser.Claims)
        { }

        public AuthenticatedUser(Guid userId, Guid organizationId, string? name,
            string? email, string? role, string? accessToken, string? refreshToken,
            long expires, string? status, string? type, IDictionary<string,
                IEnumerable<string>>? claims)
        {
            UserId = userId;
            OrganizationId = organizationId;
            Name = name;
            Email = email;
            Role = role;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Expires = expires;
            Status = status;
            Type = type;
            Claims = claims;
        }

        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public long Expires { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public IDictionary<string, IEnumerable<string>>? Claims { get; set; }

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
                new Claim(ClaimTypes.Sid, UserId.ToString()),
                new Claim(ClaimTypes.Expiration, Expires.ToString()),
                new Claim(ClaimTypes.Name, Name ?? string.Empty),
                new Claim(ClaimTypes.Role, Role ?? string.Empty),
                new Claim(ClaimTypes.Email, Email ?? string.Empty)
            };

            if (Claims != null && Claims.Any())
            {
                foreach (var claimSet in Claims)
                {
                    claims.AddRange(claimSet.Value.Select(value => new Claim(claimSet.Key, value)));
                }
            }

            return claims;
        }

        public ClaimsIdentity ToClaimsIdentity()
            => new ClaimsIdentity(ToClaimsEnumerable(), "mamey");
    }