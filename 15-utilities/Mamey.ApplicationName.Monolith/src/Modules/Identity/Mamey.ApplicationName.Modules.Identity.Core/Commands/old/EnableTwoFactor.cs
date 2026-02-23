using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record EnableTwoFactor(Guid UserId) : ICommand;