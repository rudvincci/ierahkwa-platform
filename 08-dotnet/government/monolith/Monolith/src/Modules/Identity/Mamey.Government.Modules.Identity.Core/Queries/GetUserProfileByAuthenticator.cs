using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Identity.Core.DTO;

namespace Mamey.Government.Modules.Identity.Core.Queries;

internal class GetUserProfileByAuthenticator : IQuery<UserProfileDto?>
{
    public string Issuer { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
}
