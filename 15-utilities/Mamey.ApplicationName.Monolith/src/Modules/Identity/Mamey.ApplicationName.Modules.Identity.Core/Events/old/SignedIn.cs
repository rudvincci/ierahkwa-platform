using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record SignedIn(Guid UserId, DateTime SignedInAt) : IEvent;