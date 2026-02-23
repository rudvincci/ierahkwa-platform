using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record VerifySmsTwoFactor(Guid UserId, string PhoneNumber, string Code) : ICommand;