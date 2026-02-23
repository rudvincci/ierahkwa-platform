using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Progress.Application.Exceptions;
using Pupitre.Progress.Contracts.Commands;
using Pupitre.Progress.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;

namespace Pupitre.Progress.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateLearningProgressHandler : ICommandHandler<UpdateLearningProgress>
{
    private readonly ILearningProgressRepository _learningprogressRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public UpdateLearningProgressHandler(
        ILearningProgressRepository learningprogressRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _learningprogressRepository = learningprogressRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(UpdateLearningProgress command, CancellationToken cancellationToken = default)
    {
        var learningprogress = await _learningprogressRepository.GetAsync(command.Id);

        if(learningprogress is null)
        {
            throw new LearningProgressNotFoundException(command.Id);
        }

        learningprogress.Update(command.Name, command.Tags, command.ProgramCode,
            command.CredentialType, command.CompletionDate, command.Metadata, command.Nationality);
        await _learningprogressRepository.UpdateAsync(learningprogress, cancellationToken);

        if (command.ReissueCredential)
        {
            var payload = BuildPayload(learningprogress, command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);
            if (!string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                learningprogress.AttachBlockchainReceipt(
                    receipt.IdentityId!,
                    receipt.BlockchainAccount ?? learningprogress.BlockchainAccount,
                    receipt.DocumentId,
                    receipt.DocumentHash,
                    receipt.LedgerTransactionId,
                    receipt.CredentialIssuedAt,
                    receipt.PublishedToLedger ? "issued" : "pending");
                await _learningprogressRepository.UpdateAsync(learningprogress, cancellationToken);
            }
        }

        await _eventProcessor.ProcessAsync(learningprogress.Events);
    }

    private static EducationLedgerPayload BuildPayload(Domain.Entities.LearningProgress learningprogress, UpdateLearningProgress command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = learningprogress.CitizenId,
            IdentityId = learningprogress.GovernmentIdentityId,
            BlockchainAccount = learningprogress.BlockchainAccount,
            ProgramCode = learningprogress.ProgramCode,
            CredentialType = learningprogress.CredentialType,
            CompletionDate = command.CompletionDate ?? learningprogress.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"learning-progress-{command.Id:N}",
            SourceAccount = learningprogress.BlockchainAccount,
            TargetAccount = learningprogress.BlockchainAccount,
            Nationality = command.Nationality ?? learningprogress.Nationality
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}


