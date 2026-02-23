using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Notifications.Application.Exceptions;
using Pupitre.Notifications.Contracts.Commands;
using Pupitre.Notifications.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;

namespace Pupitre.Notifications.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateNotificationHandler : ICommandHandler<UpdateNotification>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public UpdateNotificationHandler(
        INotificationRepository notificationRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _notificationRepository = notificationRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(UpdateNotification command, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetAsync(command.Id);

        if(notification is null)
        {
            throw new NotificationNotFoundException(command.Id);
        }

        notification.Update(command.Name, command.Tags, command.ProgramCode,
            command.CredentialType, command.CompletionDate, command.Metadata, command.Nationality);
        await _notificationRepository.UpdateAsync(notification, cancellationToken);

        if (command.ReissueCredential)
        {
            var payload = BuildPayload(notification, command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);
            if (!string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                notification.AttachBlockchainReceipt(
                    receipt.IdentityId!,
                    receipt.BlockchainAccount ?? notification.BlockchainAccount,
                    receipt.DocumentId,
                    receipt.DocumentHash,
                    receipt.LedgerTransactionId,
                    receipt.CredentialIssuedAt,
                    receipt.PublishedToLedger ? "issued" : "pending");
                await _notificationRepository.UpdateAsync(notification, cancellationToken);
            }
        }

        await _eventProcessor.ProcessAsync(notification.Events);
    }

    private static EducationLedgerPayload BuildPayload(Domain.Entities.Notification notification, UpdateNotification command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = notification.CitizenId,
            IdentityId = notification.GovernmentIdentityId,
            BlockchainAccount = notification.BlockchainAccount,
            ProgramCode = notification.ProgramCode,
            CredentialType = notification.CredentialType,
            CompletionDate = command.CompletionDate ?? notification.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"notification-{command.Id:N}",
            SourceAccount = notification.BlockchainAccount,
            TargetAccount = notification.BlockchainAccount,
            Nationality = command.Nationality ?? notification.Nationality
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}
