using Mamey.Constants;
using Microsoft.Graph.Models;

namespace Mamey.Graph.Requests;

public class CreateUserRequest : IGraphRequest
{
    public CreateUserRequest(bool accountEnabled, string displayName,
        string onPremisesImmutableId, string mailNickname,
        PasswordProfile passwordProfile, string userPrincipalName)
    {
        if (string.IsNullOrEmpty(displayName))
        {
            throw new ArgumentException($"'{nameof(displayName)}' cannot be null or empty.", nameof(displayName));
        }

        if (string.IsNullOrEmpty(onPremisesImmutableId))
        {
            throw new ArgumentException($"'{nameof(onPremisesImmutableId)}' cannot be null or empty.", nameof(onPremisesImmutableId));
        }

        if (string.IsNullOrEmpty(mailNickname))
        {
            throw new ArgumentException($"'{nameof(mailNickname)}' cannot be null or empty.", nameof(mailNickname));
        }

        if (string.IsNullOrEmpty(userPrincipalName))
        {
            throw new ArgumentException($"'{nameof(userPrincipalName)}' cannot be null or empty.", nameof(userPrincipalName));
        }

        if (!RegularExpressions.Email.IsMatch(userPrincipalName))
        {
            throw new ArgumentException($"{nameof(userPrincipalName)} is invalid.");
        }

        AccountEnabled = accountEnabled;
        DisplayName = displayName;
        OnPremisesImmutableId = onPremisesImmutableId;
        MailNickname = mailNickname;
        PasswordProfile = passwordProfile ?? throw new ArgumentNullException(nameof(passwordProfile));
        UserPrincipalName = userPrincipalName;
    }
    /// <summary>
    /// true if the account is enabled; otherwise, false.
    /// </summary>
    public bool AccountEnabled { get; set; }
    /// <summary>
    /// The name to display in the address book for the user.
    /// </summary>
    public string DisplayName { get; set; }
    /// <summary>
    /// Required only when creating a new user account if you are using a
    /// federated domain for the user's userPrincipalName (UPN) property.
    /// </summary>
    public string OnPremisesImmutableId { get; set; }
    /// <summary>
    /// The mail alias for the user.
    /// </summary>
    public string MailNickname { get; set; }
    /// <summary>
    /// The password profile for the user.
    /// </summary>
    public PasswordProfile PasswordProfile { get; set; }
    /// <summary>
    /// The user principal name (someuser@contoso.com). It's an Internet-style
    /// login name for the user based on the Internet standard RFC 822. By
    /// convention, this should map to the user's email name. The general format
    /// is alias@domain, where domain must be present in the tenant's collection
    /// of verified domains. The verified domains for the tenant can be accessed
    /// from the verifiedDomains property of organization.
    /// NOTE: This property cannot contain accent characters.Only the following
    /// characters are allowed A - Z, a - z, 0 - 9, ' . - _ ! # ^ ~. For the
    /// complete list of allowed characters, see username policies.
    /// </summary>
    public string UserPrincipalName { get; set; }
    
}

public interface IGraphRequest
{
    // Marker
}