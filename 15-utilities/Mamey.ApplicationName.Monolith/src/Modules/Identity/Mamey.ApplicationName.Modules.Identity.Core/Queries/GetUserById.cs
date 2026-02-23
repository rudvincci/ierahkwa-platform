using Mamey.ApplicationName.Modules.Identity.Core.DTO;
using Mamey.CQRS.Queries;

namespace Mamey.ApplicationName.Modules.Identity.Core.Queries;

internal record GetUserById(Guid UserId) : IQuery<ApplicationUserDto?>;