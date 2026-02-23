using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record ConfirmEmail(Guid UserId, string Token) : ICommand;