using System;
using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Parents.Application.Exceptions;
using Pupitre.Parents.Contracts.Commands;
using Pupitre.Parents.Domain.Entities;
using Pupitre.Parents.Domain.Repositories;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;

namespace Pupitre.Parents.Application.Commands.Handlers;

internal sealed class AddParentHandler : ICommandHandler<AddParent>
{
    private readonly IParentRepository _parentRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public AddParentHandler(IParentRepository parentRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _parentRepository = parentRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(AddParent command, CancellationToken cancellationToken = default)
    {
        
        var parent = await _parentRepository.GetAsync(command.Id);
        
        if (parent is not null)
        {
            throw new ParentAlreadyExistsException(command.Id);
        }

        parent = Parent.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags,
            citizenId: command.CitizenId, nationality: command.Nationality, programCode: command.ProgramCode,
            credentialType: command.CredentialType, completionDate: command.CompletionDate, metadata: command.Metadata);
        await _parentRepository.AddAsync(parent, cancellationToken);

        if (command.PublishToLedger)
        {
            var payload = BuildLedgerPayload(command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);

            if (string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                throw new InvalidOperationException("Blockchain identity could not be resolved for parent credential.");
            }

            parent.AttachBlockchainReceipt(
                receipt.IdentityId,
                receipt.BlockchainAccount ?? command.BlockchainAccount,
                receipt.DocumentId,
                receipt.DocumentHash,
                receipt.LedgerTransactionId,
                receipt.CredentialIssuedAt,
                receipt.PublishedToLedger ? "issued" : "pending");

            await _parentRepository.UpdateAsync(parent, cancellationToken);
        }

        await _eventProcessor.ProcessAsync(parent.Events);
    }

    private static EducationLedgerPayload BuildLedgerPayload(AddParent command)
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
            TransactionId = $"parent-{command.Id:N}",
            SourceAccount = command.BlockchainAccount,
            TargetAccount = command.BlockchainAccount
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}

