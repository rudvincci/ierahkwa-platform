using Mamey.CQRS.Events;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record SignedUp(Guid UserId, string Email, string Role, Name Name, string ConfirmUrl) : IEvent;