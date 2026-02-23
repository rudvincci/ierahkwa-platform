using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record AddRoleToUser(Guid UserId, string RoleName) : ICommand;