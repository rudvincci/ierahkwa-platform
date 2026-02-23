using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;
using Pupitre.GLEs.Application.Exceptions;
using Pupitre.GLEs.Contracts.Commands;
using Pupitre.GLEs.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.GLEs.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateGLEHandler : ICommandHandler<UpdateGLE>
{
    private readonly IGLERepository _gleRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public UpdateGLEHandler(
        IGLERepository gleRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _gleRepository = gleRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(UpdateGLE command, CancellationToken cancellationToken = default)
    {
        var gle = await _gleRepository.GetAsync(command.Id);

        if(gle is null)
        {
            throw new GLENotFoundException(command.Id);
        }

        gle.Update(command.Name, command.Tags, command.ProgramCode,
            command.CredentialType, command.CompletionDate, command.Metadata, command.Nationality);
        await _gleRepository.UpdateAsync(gle, cancellationToken);

        if (command.ReissueCredential)
        {
            var payload = BuildPayload(gle, command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);
            if (!string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                gle.AttachBlockchainReceipt(
                    receipt.IdentityId!,
                    receipt.BlockchainAccount ?? gle.BlockchainAccount,
                    receipt.DocumentId,
                    receipt.DocumentHash,
                    receipt.LedgerTransactionId,
                    receipt.CredentialIssuedAt,
                    receipt.PublishedToLedger ? "issued" : "pending");
                await _gleRepository.UpdateAsync(gle, cancellationToken);
            }
        }

        await _eventProcessor.ProcessAsync(gle.Events);
    }

    private static EducationLedgerPayload BuildPayload(Pupitre.GLEs.Domain.Entities.GLE gle, UpdateGLE command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = gle.CitizenId,
            IdentityId = gle.GovernmentIdentityId,
            BlockchainAccount = gle.BlockchainAccount,
            ProgramCode = gle.ProgramCode,
            CredentialType = gle.CredentialType,
            CompletionDate = command.CompletionDate ?? gle.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"gle-{command.Id:N}",
            SourceAccount = gle.BlockchainAccount,
            TargetAccount = gle.BlockchainAccount,
            Nationality = command.Nationality ?? gle.Nationality
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}
