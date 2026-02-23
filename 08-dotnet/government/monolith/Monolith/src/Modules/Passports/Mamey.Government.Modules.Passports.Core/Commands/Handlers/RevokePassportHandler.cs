using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Passports.Core.Commands.Handlers;

internal sealed class RevokePassportHandler : ICommandHandler<RevokePassport>
{
    private readonly IPassportRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<RevokePassportHandler> _logger;

    public RevokePassportHandler(
        IPassportRepository repository,
        IMessageBroker messageBroker,
        ILogger<RevokePassportHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(RevokePassport command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoking passport {PassportId}", command.PassportId);

        var passport = await _repository.GetAsync(new PassportId(command.PassportId), cancellationToken);
        if (passport is null)
        {
            throw new InvalidOperationException($"Passport with ID {command.PassportId} not found");
        }

        if (!passport.IsActive)
        {
            throw new InvalidOperationException($"Passport {command.PassportId} is already revoked");
        }

        passport.Revoke(command.Reason);

        await _repository.UpdateAsync(passport, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new PassportRevokedEvent(passport.Id.Value, passport.CitizenId, command.Reason),
            cancellationToken);

        _logger.LogInformation("Passport {PassportId} revoked. Reason: {Reason}",
            command.PassportId, command.Reason);
    }
}
