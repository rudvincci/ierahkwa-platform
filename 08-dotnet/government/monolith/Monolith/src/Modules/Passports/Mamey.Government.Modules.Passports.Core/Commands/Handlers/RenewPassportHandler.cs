using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Passports.Core.Commands.Handlers;

internal sealed class RenewPassportHandler : ICommandHandler<RenewPassport>
{
    private readonly IPassportRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<RenewPassportHandler> _logger;

    public RenewPassportHandler(
        IPassportRepository repository,
        IMessageBroker messageBroker,
        ILogger<RenewPassportHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(RenewPassport command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Renewing passport {PassportId}", command.PassportId);

        var passport = await _repository.GetAsync(new PassportId(command.PassportId), cancellationToken);
        if (passport is null)
        {
            throw new InvalidOperationException($"Passport with ID {command.PassportId} not found");
        }

        if (!passport.IsActive)
        {
            throw new InvalidOperationException($"Cannot renew revoked passport {command.PassportId}");
        }

        var newExpiryDate = DateTime.UtcNow.AddYears(command.ValidityYears);
        passport.Renew(newExpiryDate);

        await _repository.UpdateAsync(passport, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new PassportRenewedEvent(passport.Id.Value, passport.CitizenId, newExpiryDate),
            cancellationToken);

        _logger.LogInformation("Passport {PassportId} renewed until {ExpiryDate}",
            command.PassportId, newExpiryDate);
    }
}
