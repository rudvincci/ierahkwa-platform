using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Passports.Core.Clients;
using Mamey.Government.Modules.Passports.Core.Domain.Entities;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Events;
using Mamey.Government.Modules.Passports.Core.Services;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Passports.Core.Commands.Handlers;

internal sealed class IssuePassportHandler : ICommandHandler<IssuePassport>
{
    private readonly IPassportRepository _repository;
    private readonly ICitizensClient _citizensClient;
    private readonly IMrzGeneratorService _mrzGenerator;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<IssuePassportHandler> _logger;

    public IssuePassportHandler(
        IPassportRepository repository,
        ICitizensClient citizensClient,
        IMrzGeneratorService mrzGenerator,
        IMessageBroker messageBroker,
        ILogger<IssuePassportHandler> logger)
    {
        _repository = repository;
        _citizensClient = citizensClient;
        _mrzGenerator = mrzGenerator;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(IssuePassport command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Issuing passport for citizen {CitizenId}", command.CitizenId);
        
        // Get citizen details for MRZ
        var citizen = await _citizensClient.GetAsync(command.CitizenId);
        if (citizen is null)
        {
            throw new InvalidOperationException($"Citizen with ID {command.CitizenId} not found");
        }

        // Generate passport number
        var passportNumber = GeneratePassportNumber();
        var issuedDate = DateTime.UtcNow;
        var expiryDate = issuedDate.AddYears(command.ValidityYears);

        // Generate MRZ
        var mrzInput = new MrzInput(
            DocumentCode: "P",
            IssuingState: "FWM", // FutureWampum
            Surname: citizen.LastName?.ToUpperInvariant() ?? string.Empty,
            GivenNames: citizen.FirstName?.ToUpperInvariant() ?? string.Empty,
            PassportNumber: passportNumber.Value,
            Nationality: "FWM",
            DateOfBirth: citizen.DateOfBirth,
            Sex: "X", // Unspecified
            ExpirationDate: expiryDate);

        var mrz = _mrzGenerator.GenerateTd3Mrz(mrzInput);

        // Create passport
        var passport = new Passport(
            new PassportId(command.Id),
            new TenantId(command.TenantId),
            command.CitizenId,
            passportNumber,
            issuedDate,
            expiryDate,
            mrz.FullMrz);

        await _repository.AddAsync(passport, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new PassportIssuedEvent(passport.Id.Value, command.CitizenId, passportNumber.Value),
            cancellationToken);

        _logger.LogInformation("Passport {PassportNumber} issued for citizen {CitizenId}",
            passportNumber.Value, command.CitizenId);
    }

    private static PassportNumber GeneratePassportNumber()
    {
        // Generate passport number: P + 8 digits
        var random = new Random();
        var number = random.Next(10000000, 99999999);
        return new PassportNumber($"P{number}");
    }
}
