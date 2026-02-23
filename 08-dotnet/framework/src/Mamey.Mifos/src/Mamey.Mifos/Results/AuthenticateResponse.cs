using System.Text.Json.Serialization;
using Mamey.Mifos.Entities;

namespace Mamey.Mifos.Results;

public class AuthenticateResponse : IMifosResponse
{
    public AuthenticateResponse()
    {
    }

    [JsonConstructor]
    public AuthenticateResponse(string username, int userId, string base64EncodedAuthenticationKey,
        bool authenticated, int officeId, string officeName, IEnumerable<Role> roles,
        IEnumerable<string> permissions, bool shouldRenewPassword,
        bool isTwoFactorAuthenticationRequired)
    {
        Username = username;
        UserId = userId;
        Base64EncodedAuthenticationKey = base64EncodedAuthenticationKey;
        Authenticated = authenticated;
        OfficeId = officeId;
        OfficeName = officeName;
        Roles = roles;
        Permissions = permissions;
        ShouldRenewPassword = shouldRenewPassword;
        IsTwoFactorAuthenticationRequired = isTwoFactorAuthenticationRequired;
    }

    public string Username { get; set; }
    public int UserId { get; set; }
    public string Base64EncodedAuthenticationKey { get; set; }
    public bool Authenticated { get; set; }
    public int OfficeId { get; set; }
    public string OfficeName { get; set; }
    public IEnumerable<Role> Roles { get; set; }
    public IEnumerable<string> Permissions { get; set; }
    public bool ShouldRenewPassword { get; set; }
    public bool IsTwoFactorAuthenticationRequired { get; set; }
}

