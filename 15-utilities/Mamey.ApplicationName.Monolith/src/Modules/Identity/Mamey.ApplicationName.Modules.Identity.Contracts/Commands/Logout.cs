using Mamey.CQRS.Commands;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Identity.Contracts.Commands;

public class Logout(UserId userId) : ICommand;