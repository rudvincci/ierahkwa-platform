using Mamey.Types;

namespace Mamey.FWID.Notifications.Application.Templates.Models;

/// <summary>
/// Email template model for DID creation notifications.
/// </summary>
internal record DIDCreatedDto(Name Name, string DidString) : IEmailTemplate;







