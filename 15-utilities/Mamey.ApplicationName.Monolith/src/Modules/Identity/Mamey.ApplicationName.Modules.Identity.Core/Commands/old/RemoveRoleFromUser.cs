using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record RemoveRoleFromUser(Guid UserId, string RoleName) : ICommand;