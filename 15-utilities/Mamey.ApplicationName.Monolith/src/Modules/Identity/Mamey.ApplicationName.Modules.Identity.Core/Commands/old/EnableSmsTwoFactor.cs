using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record EnableSmsTwoFactor(Guid UserId, string PhoneNumber) : ICommand;