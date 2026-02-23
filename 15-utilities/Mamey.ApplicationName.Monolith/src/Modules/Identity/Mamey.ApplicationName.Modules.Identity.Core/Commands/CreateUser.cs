using Mamey.CQRS.Commands;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record CreateUser(string UserName, string Email, string Password, Name Name, IEnumerable<string> Roles, bool RegistrationComplete = false, bool ConfirmEmail = true, bool ConfirmPhone = true) : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
internal record UpdateUser(UserId UserId, string UserName, string Email, string Password, Name Name) : ICommand;