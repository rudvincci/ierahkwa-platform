using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;
using Pupitre.Educators.Application.Exceptions;
using Pupitre.Educators.Contracts.Commands;
using Pupitre.Educators.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Educators.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateEducatorHandler : ICommandHandler<UpdateEducator>
{
    private readonly IEducatorRepository _educatorRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public UpdateEducatorHandler(
        IEducatorRepository educatorRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _educatorRepository = educatorRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(UpdateEducator command, CancellationToken cancellationToken = default)
    {
        var educator = await _educatorRepository.GetAsync(command.Id);

        if(educator is null)
        {
            throw new EducatorNotFoundException(command.Id);
        }

        educator.Update(command.Name, command.Tags, command.ProgramCode,
            command.CredentialType, command.CompletionDate, command.Metadata, command.Nationality);
        await _educatorRepository.UpdateAsync(educator, cancellationToken);

        if (command.ReissueCredential)
        {
            var payload = BuildPayload(educator, command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);
            if (!string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                educator.AttachBlockchainReceipt(
                    receipt.IdentityId!,
                    receipt.BlockchainAccount ?? educator.BlockchainAccount,
                    receipt.DocumentId,
                    receipt.DocumentHash,
                    receipt.LedgerTransactionId,
                    receipt.CredentialIssuedAt,
                    receipt.PublishedToLedger ? "issued" : "pending");
                await _educatorRepository.UpdateAsync(educator, cancellationToken);
            }
        }

        await _eventProcessor.ProcessAsync(educator.Events);
    }

    private static EducationLedgerPayload BuildPayload(Pupitre.Educators.Domain.Entities.Educator educator, UpdateEducator command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = educator.CitizenId,
            IdentityId = educator.GovernmentIdentityId,
            BlockchainAccount = educator.BlockchainAccount,
            ProgramCode = educator.ProgramCode,
            CredentialType = educator.CredentialType,
            CompletionDate = command.CompletionDate ?? educator.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"educator-{command.Id:N}",
            SourceAccount = educator.BlockchainAccount,
            TargetAccount = educator.BlockchainAccount,
            Nationality = command.Nationality ?? educator.Nationality
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}
