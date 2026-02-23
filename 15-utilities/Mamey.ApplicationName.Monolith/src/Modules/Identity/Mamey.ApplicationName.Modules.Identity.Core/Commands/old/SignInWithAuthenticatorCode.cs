using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record SignInWithAuthenticatorCode(Guid UserId, string Code) : ICommand;