using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record ConfirmPasswordReset(Guid UserId, string Token, string NewPassword) : ICommand; 