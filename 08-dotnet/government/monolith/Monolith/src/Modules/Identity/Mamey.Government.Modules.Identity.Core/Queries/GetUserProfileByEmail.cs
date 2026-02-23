using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Identity.Core.DTO;

namespace Mamey.Government.Modules.Identity.Core.Queries;

internal class GetUserProfileByEmail : IQuery<UserProfileDto?>
{
    public string Email { get; set; } = string.Empty;
}
