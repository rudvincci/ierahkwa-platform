using System;
using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Saga.Api.Messages;

internal record AddFunds(Guid WalletId, string Currency, decimal Amount, string TransferName = null,
    string TransferMetadata = null) : ICommand
{
    public Guid TransferId { get; init; } = Guid.NewGuid();
}
