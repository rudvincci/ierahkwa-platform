using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Rewards.Application.Exceptions;
using Pupitre.Rewards.Contracts.Commands;
using Pupitre.Rewards.Domain.Repositories;

namespace Pupitre.Rewards.Application.Commands.Handlers;

internal sealed class DeleteRewardHandler : ICommandHandler<DeleteReward>
{
    private readonly IRewardRepository _rewardRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteRewardHandler(IRewardRepository rewardRepository, 
    IEventProcessor eventProcessor)
    {
        _rewardRepository = rewardRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteReward command, CancellationToken cancellationToken = default)
    {
        var reward = await _rewardRepository.GetAsync(command.Id, cancellationToken);

        if (reward is null)
        {
            throw new RewardNotFoundException(command.Id);
        }

        await _rewardRepository.DeleteAsync(reward.Id);
        await _eventProcessor.ProcessAsync(reward.Events);
    }
}


