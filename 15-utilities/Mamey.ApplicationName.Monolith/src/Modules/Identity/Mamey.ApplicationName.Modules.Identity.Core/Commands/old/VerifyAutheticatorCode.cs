using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record VerifyAutheticatorCode(Guid UserId, string Code) : ICommand;