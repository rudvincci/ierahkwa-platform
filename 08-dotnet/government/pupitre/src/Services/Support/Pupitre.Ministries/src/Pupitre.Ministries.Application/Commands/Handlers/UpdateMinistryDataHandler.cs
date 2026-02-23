using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Ministries.Application.Exceptions;
using Pupitre.Ministries.Contracts.Commands;
using Pupitre.Ministries.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;

namespace Pupitre.Ministries.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateMinistryDataHandler : ICommandHandler<UpdateMinistryData>
{
    private readonly IMinistryDataRepository _ministrydataRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public UpdateMinistryDataHandler(
        IMinistryDataRepository ministrydataRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _ministrydataRepository = ministrydataRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(UpdateMinistryData command, CancellationToken cancellationToken = default)
    {
        var ministrydata = await _ministrydataRepository.GetAsync(command.Id);

        if(ministrydata is null)
        {
            throw new MinistryDataNotFoundException(command.Id);
        }

        ministrydata.Update(command.Name, command.Tags, command.ProgramCode, command.CredentialType,
            command.CompletionDate, command.Metadata);
        await _ministrydataRepository.UpdateAsync(ministrydata);

        if (command.ReissueCredential)
        {
            var payload = new EducationLedgerPayload
            {
                MinistryDataId = command.Id,
                CitizenId = ministrydata.CitizenId,
                IdentityId = ministrydata.GovernmentIdentityId,
                BlockchainAccount = ministrydata.BlockchainAccount,
                ProgramCode = ministrydata.ProgramCode,
                CredentialType = ministrydata.CredentialType,
                CompletionDate = command.CompletionDate ?? ministrydata.CompletionDate,
                CredentialDocumentBase64 = command.CredentialDocumentBase64,
                CredentialMimeType = command.CredentialMimeType,
                Metadata = ToReadOnly(command.Metadata)
            };

            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);
            if (!string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                ministrydata.AttachBlockchainReceipt(
                    receipt.IdentityId!,
                    receipt.BlockchainAccount ?? ministrydata.BlockchainAccount,
                    receipt.DocumentId,
                    receipt.DocumentHash,
                    receipt.LedgerTransactionId,
                    receipt.CredentialIssuedAt,
                    receipt.PublishedToLedger ? "issued" : "pending");
                await _ministrydataRepository.UpdateAsync(ministrydata, cancellationToken);
            }
        }

        await _eventProcessor.ProcessAsync(ministrydata.Events);
    }

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}


