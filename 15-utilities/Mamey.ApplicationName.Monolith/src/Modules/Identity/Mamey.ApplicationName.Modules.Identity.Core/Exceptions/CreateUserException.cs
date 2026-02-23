using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Identity.Core.Exceptions;

internal sealed class CreateUserException(string reason) : MameyException(reason);