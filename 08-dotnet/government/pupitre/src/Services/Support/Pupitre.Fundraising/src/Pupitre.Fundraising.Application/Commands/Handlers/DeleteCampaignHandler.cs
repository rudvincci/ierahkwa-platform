using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Fundraising.Application.Exceptions;
using Pupitre.Fundraising.Contracts.Commands;
using Pupitre.Fundraising.Domain.Repositories;

namespace Pupitre.Fundraising.Application.Commands.Handlers;

internal sealed class DeleteCampaignHandler : ICommandHandler<DeleteCampaign>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteCampaignHandler(ICampaignRepository campaignRepository, 
    IEventProcessor eventProcessor)
    {
        _campaignRepository = campaignRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteCampaign command, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetAsync(command.Id, cancellationToken);

        if (campaign is null)
        {
            throw new CampaignNotFoundException(command.Id);
        }

        await _campaignRepository.DeleteAsync(campaign.Id);
        await _eventProcessor.ProcessAsync(campaign.Events);
    }
}


