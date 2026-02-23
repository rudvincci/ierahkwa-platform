using System;
using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.GLEs.Application.Exceptions;
using Pupitre.GLEs.Contracts.Commands;
using Pupitre.GLEs.Domain.Entities;
using Pupitre.GLEs.Domain.Repositories;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;

namespace Pupitre.GLEs.Application.Commands.Handlers;

internal sealed class AddGLEHandler : ICommandHandler<AddGLE>
{
    private readonly IGLERepository _gleRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public AddGLEHandler(IGLERepository gleRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _gleRepository = gleRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(AddGLE command, CancellationToken cancellationToken = default)
    {
        
        var gle = await _gleRepository.GetAsync(command.Id);
        
        if(gle is not null)
        {
            throw new GLEAlreadyExistsException(command.Id);
        }

        gle = GLE.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags,
            citizenId: command.CitizenId, nationality: command.Nationality, programCode: command.ProgramCode,
            credentialType: command.CredentialType, completionDate: command.CompletionDate, metadata: command.Metadata);
        await _gleRepository.AddAsync(gle, cancellationToken);

        if (command.PublishToLedger)
        {
            var payload = BuildLedgerPayload(command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);

            if (string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                throw new InvalidOperationException("Blockchain identity could not be resolved for GLE credential.");
            }

            gle.AttachBlockchainReceipt(
                receipt.IdentityId,
                receipt.BlockchainAccount ?? command.BlockchainAccount,
                receipt.DocumentId,
                receipt.DocumentHash,
                receipt.LedgerTransactionId,
                receipt.CredentialIssuedAt,
                receipt.PublishedToLedger ? "issued" : "pending");

            await _gleRepository.UpdateAsync(gle, cancellationToken);
        }

        await _eventProcessor.ProcessAsync(gle.Events);
    }

    private static EducationLedgerPayload BuildLedgerPayload(AddGLE command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = command.CitizenId,
            IdentityId = command.IdentityId,
            BlockchainAccount = command.BlockchainAccount,
            FirstName = command.FirstName,
            LastName = command.LastName,
            DateOfBirth = command.DateOfBirth,
            Nationality = command.Nationality,
            ProgramCode = command.ProgramCode,
            CredentialType = command.CredentialType,
            CompletionDate = command.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"gle-{command.Id:N}",
            SourceAccount = command.BlockchainAccount,
            TargetAccount = command.BlockchainAccount
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}

