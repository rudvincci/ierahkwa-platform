using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Saga.Api.Messages.Passports;

internal record PassportRenewalRequested(Guid PassportId, Guid CitizenId) : IEvent;
