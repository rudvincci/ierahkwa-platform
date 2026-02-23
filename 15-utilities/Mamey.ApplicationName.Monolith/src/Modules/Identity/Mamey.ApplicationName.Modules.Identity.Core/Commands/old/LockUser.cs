using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record LockUser(Guid UserId, DateTimeOffset? LockoutEnd) : ICommand;