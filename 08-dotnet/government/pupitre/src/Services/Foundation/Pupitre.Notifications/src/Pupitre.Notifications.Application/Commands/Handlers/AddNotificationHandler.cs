using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Notifications.Application.Exceptions;
using Pupitre.Notifications.Contracts.Commands;
using Pupitre.Notifications.Domain.Entities;
using Pupitre.Notifications.Domain.Repositories;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;

namespace Pupitre.Notifications.Application.Commands.Handlers;

internal sealed class AddNotificationHandler : ICommandHandler<AddNotification>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public AddNotificationHandler(INotificationRepository notificationRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _notificationRepository = notificationRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(AddNotification command, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetAsync(command.Id);

        if (notification is not null)
        {
            throw new NotificationAlreadyExistsException(command.Id);
        }

        notification = Notification.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags,
            citizenId: command.CitizenId, nationality: command.Nationality, programCode: command.ProgramCode,
            credentialType: command.CredentialType, completionDate: command.CompletionDate, metadata: command.Metadata);
        await _notificationRepository.AddAsync(notification, cancellationToken);

        if (command.PublishToLedger)
        {
            var payload = BuildLedgerPayload(command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);

            if (string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                throw new InvalidOperationException("Blockchain identity could not be resolved for notification credential.");
            }

            notification.AttachBlockchainReceipt(
                receipt.IdentityId,
                receipt.BlockchainAccount ?? command.BlockchainAccount,
                receipt.DocumentId,
                receipt.DocumentHash,
                receipt.LedgerTransactionId,
                receipt.CredentialIssuedAt,
                receipt.PublishedToLedger ? "issued" : "pending");

            await _notificationRepository.UpdateAsync(notification, cancellationToken);
        }

        await _eventProcessor.ProcessAsync(notification.Events);
    }

    private static EducationLedgerPayload BuildLedgerPayload(AddNotification command)
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
            TransactionId = $"notification-{command.Id:N}",
            SourceAccount = command.BlockchainAccount,
            TargetAccount = command.BlockchainAccount
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}

