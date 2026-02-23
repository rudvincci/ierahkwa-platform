using System;
using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Saga.Api.Messages;

internal record FundsAdded(Guid WalletId, Guid OwnerId, string Currency, decimal Amount, string TransferName = null,
    string TransferMetadata = null) : IEvent;
