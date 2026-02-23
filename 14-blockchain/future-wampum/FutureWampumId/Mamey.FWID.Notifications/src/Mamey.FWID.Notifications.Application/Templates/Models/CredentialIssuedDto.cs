using Mamey.Types;

namespace Mamey.FWID.Notifications.Application.Templates.Models;

/// <summary>
/// Email template model for credential issuance notifications.
/// </summary>
internal record CredentialIssuedDto(Name Name, string CredentialType) : IEmailTemplate;







