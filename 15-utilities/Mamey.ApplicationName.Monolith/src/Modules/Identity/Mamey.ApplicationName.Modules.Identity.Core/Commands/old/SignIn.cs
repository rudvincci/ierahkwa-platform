using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record SignIn(string Email, string Password, bool RememberMe = false) : ICommand;