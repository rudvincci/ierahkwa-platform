using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Services;
using Pupitre.Rewards.Application.Exceptions;
using Pupitre.Rewards.Contracts.Commands;
using Pupitre.Rewards.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Rewards.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateRewardHandler : ICommandHandler<UpdateReward>
{
    private readonly IRewardRepository _rewardRepository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IEducationLedgerService _educationLedgerService;

    public UpdateRewardHandler(
        IRewardRepository rewardRepository,
        IEventProcessor eventProcessor,
        IEducationLedgerService educationLedgerService)
    {
        _rewardRepository = rewardRepository;
        _eventProcessor = eventProcessor;
        _educationLedgerService = educationLedgerService;
    }

    public async Task HandleAsync(UpdateReward command, CancellationToken cancellationToken = default)
    {
        var reward = await _rewardRepository.GetAsync(command.Id);

        if(reward is null)
        {
            throw new RewardNotFoundException(command.Id);
        }

        reward.Update(command.Name, command.Tags, command.ProgramCode,
            command.CredentialType, command.CompletionDate, command.Metadata, command.Nationality);
        await _rewardRepository.UpdateAsync(reward, cancellationToken);

        if (command.ReissueCredential)
        {
            var payload = BuildPayload(reward, command);
            var receipt = await _educationLedgerService.PublishCredentialAsync(payload, cancellationToken);
            if (!string.IsNullOrWhiteSpace(receipt.IdentityId))
            {
                reward.AttachBlockchainReceipt(
                    receipt.IdentityId!,
                    receipt.BlockchainAccount ?? reward.BlockchainAccount,
                    receipt.DocumentId,
                    receipt.DocumentHash,
                    receipt.LedgerTransactionId,
                    receipt.CredentialIssuedAt,
                    receipt.PublishedToLedger ? "issued" : "pending");
                await _rewardRepository.UpdateAsync(reward, cancellationToken);
            }
        }

        await _eventProcessor.ProcessAsync(reward.Events);
    }

    private static EducationLedgerPayload BuildPayload(Domain.Entities.Reward reward, UpdateReward command)
        => new()
        {
            MinistryDataId = command.Id,
            CitizenId = reward.CitizenId,
            IdentityId = reward.GovernmentIdentityId,
            BlockchainAccount = reward.BlockchainAccount,
            ProgramCode = reward.ProgramCode,
            CredentialType = reward.CredentialType,
            CompletionDate = command.CompletionDate ?? reward.CompletionDate,
            CredentialDocumentBase64 = command.CredentialDocumentBase64,
            CredentialMimeType = command.CredentialMimeType,
            Metadata = ToReadOnly(command.Metadata),
            TransactionId = $"reward-{command.Id:N}",
            SourceAccount = reward.BlockchainAccount,
            TargetAccount = reward.BlockchainAccount,
            Nationality = command.Nationality ?? reward.Nationality
        };

    private static IReadOnlyDictionary<string, string>? ToReadOnly(IDictionary<string, string>? metadata)
        => metadata is { Count: > 0 }
            ? new Dictionary<string, string>(metadata)
            : null;
}
