using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Saga.Api.Messages.Citizenship;

internal record ApplicationValidated(Guid ApplicationId) : IEvent;
