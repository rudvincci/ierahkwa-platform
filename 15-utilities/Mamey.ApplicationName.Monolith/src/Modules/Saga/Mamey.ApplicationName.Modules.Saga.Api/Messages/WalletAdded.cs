using System;
using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Saga.Api.Messages;

internal record WalletAdded(Guid WalletId, Guid OwnerId, string Currency) : IEvent;