using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;
using Pupitre.Parents.Application.Exceptions;
using Pupitre.Parents.Contracts.Commands;
using Pupitre.Parents.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Parents.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateParentHandler : ICommandHandler<UpdateParent>
{
    private readonly IParentRepository _parentRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public UpdateParentHandler(
        IParentRepository parentRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _parentRepository = parentRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(UpdateParent command, CancellationToken cancellationToken = default)
    {
        var parent = await _parentRepository.GetAsync(command.Id);

        if (parent is null)
        {
            throw new ParentNotFoundException(command.Id);
        }

        parent.Update(command.Name, command.Tags, command.ProgramCode,
            command.CredentialType, command.CompletionDate, command.Metadata, command.Nationality);
        await _parentRepository.UpdateAsync(parent, cancellationToken);

        if (command.ReissueCredential)
        {
            var payload = BuildPayload(parent, command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);
            if (!string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                parent.AttachBlockchainReceipt(
                    receipt.IdentityId!,
                    receipt.BlockchainAccount ?? parent.BlockchainAccount,
                    receipt.DocumentId,
                    receipt.DocumentHash,
                    receipt.LedgerTransactionId,
                    receipt.CredentialIssuedAt,
                    receipt.PublishedToLedger ? "issued" : "pending");
                await _parentRepository.UpdateAsync(parent, cancellationToken);
            }
        }

        await _eventProcessor.ProcessAsync(parent.Events);
    }

    private static EducationLedgerPayload BuildPayload(Pupitre.Parents.Domain.Entities.Parent parent, UpdateParent command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = parent.CitizenId,
            IdentityId = parent.GovernmentIdentityId,
            BlockchainAccount = parent.BlockchainAccount,
            ProgramCode = parent.ProgramCode,
            CredentialType = parent.CredentialType,
            CompletionDate = command.CompletionDate ?? parent.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"parent-{command.Id:N}",
            SourceAccount = parent.BlockchainAccount,
            TargetAccount = parent.BlockchainAccount,
            Nationality = command.Nationality ?? parent.Nationality
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}
