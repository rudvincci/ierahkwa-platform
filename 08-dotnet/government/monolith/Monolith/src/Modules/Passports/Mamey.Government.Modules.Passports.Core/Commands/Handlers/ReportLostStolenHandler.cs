using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Passports.Core.Commands.Handlers;

internal sealed class ReportLostStolenHandler : ICommandHandler<ReportLostStolen>
{
    private readonly IPassportRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<ReportLostStolenHandler> _logger;

    public ReportLostStolenHandler(
        IPassportRepository repository,
        IMessageBroker messageBroker,
        ILogger<ReportLostStolenHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(ReportLostStolen command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Reporting passport {PassportId} as {ReportType}", 
            command.PassportId, command.ReportType);

        var passport = await _repository.GetAsync(new PassportId(command.PassportId), cancellationToken);
        if (passport is null)
        {
            throw new InvalidOperationException($"Passport with ID {command.PassportId} not found");
        }

        var reason = $"{command.ReportType}: {command.Description}";
        passport.Revoke(reason);

        await _repository.UpdateAsync(passport, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new PassportLostStolenEvent(
                passport.Id.Value, 
                passport.CitizenId, 
                command.ReportType, 
                command.Description),
            cancellationToken);

        _logger.LogInformation("Passport {PassportId} reported as {ReportType} and revoked",
            command.PassportId, command.ReportType);
    }
}
