using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

internal record RequestNewAccessToken(Guid UserId, string RefreshToken) : ICommand;