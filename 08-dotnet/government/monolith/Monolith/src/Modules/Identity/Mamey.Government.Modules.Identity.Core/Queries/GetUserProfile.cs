using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Identity.Core.DTO;

namespace Mamey.Government.Modules.Identity.Core.Queries;

internal class GetUserProfile : IQuery<UserProfileDto?>
{
    public Guid UserId { get; set; }
}
