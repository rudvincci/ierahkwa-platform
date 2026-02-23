using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record ChangePassword(Guid UserId, string CurrentPassword, string NewPassword) : ICommand;