using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Identity.Core.Events;

public record UserLoginRecordedEvent(Guid UserId, DateTime LoginAt) : IEvent;
