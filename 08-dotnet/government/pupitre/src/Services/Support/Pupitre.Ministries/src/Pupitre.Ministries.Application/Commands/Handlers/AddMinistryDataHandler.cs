using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Ministries.Application.Exceptions;
using Pupitre.Ministries.Contracts.Commands;
using Pupitre.Ministries.Domain.Entities;
using Pupitre.Ministries.Domain.Repositories;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;

namespace Pupitre.Ministries.Application.Commands.Handlers;

internal sealed class AddMinistryDataHandler : ICommandHandler<AddMinistryData>
{
    private readonly IMinistryDataRepository _ministrydataRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public AddMinistryDataHandler(IMinistryDataRepository ministrydataRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _ministrydataRepository = ministrydataRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(AddMinistryData command, CancellationToken cancellationToken = default)
    {
        
        var ministrydata = await _ministrydataRepository.GetAsync(command.Id);
        
        if(ministrydata is not null)
        {
            throw new MinistryDataAlreadyExistsException(command.Id);
        }

        ministrydata = MinistryData.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags,
            citizenId: command.CitizenId, programCode: command.ProgramCode, credentialType: command.CredentialType,
            completionDate: command.CompletionDate, metadata: command.Metadata, nationality: command.Nationality);
        await _ministrydataRepository.AddAsync(ministrydata, cancellationToken);

        if (command.PublishToLedger)
        {
            var payload = BuildLedgerPayload(command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);

            if (string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                throw new InvalidOperationException("Blockchain identity could not be resolved for ministry credential.");
            }

            ministrydata.AttachBlockchainReceipt(
                receipt.IdentityId,
                receipt.BlockchainAccount ?? command.BlockchainAccount,
                receipt.DocumentId,
                receipt.DocumentHash,
                receipt.LedgerTransactionId,
                receipt.CredentialIssuedAt,
                receipt.PublishedToLedger ? "issued" : "pending");

            await _ministrydataRepository.UpdateAsync(ministrydata, cancellationToken);
        }

        await _eventProcessor.ProcessAsync(ministrydata.Events);
    }

    private static EducationLedgerPayload BuildLedgerPayload(AddMinistryData command)
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
            Metadata = PublishMetadata(command.Metadata),
            TransactionId = $"edu-{command.Id:N}",
            SourceAccount = command.BlockchainAccount,
            TargetAccount = command.BlockchainAccount
        };

    private static IReadOnlyDictionary<string, string>? PublishMetadata(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}

