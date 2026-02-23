using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.TravelIdentities.Core.Clients;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.Events;
using Mamey.Government.Modules.TravelIdentities.Core.Services;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.TravelIdentities.Core.Commands.Handlers;

internal sealed class IssueTravelIdentityHandler : ICommandHandler<IssueTravelIdentity>
{
    private readonly ITravelIdentityRepository _repository;
    private readonly ICitizensClient _citizensClient;
    private readonly IAamvaBarcodeService _barcodeService;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<IssueTravelIdentityHandler> _logger;

    public IssueTravelIdentityHandler(
        ITravelIdentityRepository repository,
        ICitizensClient citizensClient,
        IAamvaBarcodeService barcodeService,
        IDocumentNumberService documentNumberService,
        IMessageBroker messageBroker,
        ILogger<IssueTravelIdentityHandler> logger)
    {
        _repository = repository;
        _citizensClient = citizensClient;
        _barcodeService = barcodeService;
        _documentNumberService = documentNumberService;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(IssueTravelIdentity command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Issuing travel identity for citizen {CitizenId}", command.CitizenId);
        
        // Get citizen details for barcode
        var citizen = await _citizensClient.GetAsync(command.CitizenId);
        if (citizen is null)
        {
            throw new InvalidOperationException($"Citizen with ID {command.CitizenId} not found");
        }

        // Generate document number
        var tenantId = new TenantId(command.TenantId);
        var documentNumber = await _documentNumberService.GenerateAsync(command.TenantId, "TID", cancellationToken);
        var travelIdentityNumber = new TravelIdentityNumber(documentNumber);
        
        var issuedDate = DateTime.UtcNow;
        var expiryDate = issuedDate.AddYears(command.ValidityYears);

        // Generate AAMVA PDF417 barcode data
        var barcodeInput = new AamvaInput(
            DocumentNumber: documentNumber,
            Jurisdiction: "NT", // National Territory
            FirstName: citizen.FirstName ?? string.Empty,
            LastName: citizen.LastName ?? string.Empty,
            MiddleName: null,
            DateOfBirth: citizen.DateOfBirth,
            Sex: "U", // Unknown - would come from citizen record
            EyeColor: "UNK",
            HeightInches: 0,
            StreetAddress: string.Empty,
            City: string.Empty,
            State: "NT",
            PostalCode: string.Empty,
            IssueDate: issuedDate,
            ExpirationDate: expiryDate,
            DocumentClass: "ID");
        
        var barcodeData = _barcodeService.GenerateBarcodeData(barcodeInput);

        // Create travel identity
        var travelIdentity = new TravelIdentity(
            new TravelIdentityId(command.Id),
            tenantId,
            command.CitizenId,
            travelIdentityNumber,
            issuedDate,
            expiryDate,
            barcodeData.RawData);

        await _repository.AddAsync(travelIdentity, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new TravelIdentityIssuedEvent(travelIdentity.Id.Value, command.CitizenId, documentNumber),
            cancellationToken);

        _logger.LogInformation("Travel identity {Number} issued for citizen {CitizenId}",
            documentNumber, command.CitizenId);
    }
}
