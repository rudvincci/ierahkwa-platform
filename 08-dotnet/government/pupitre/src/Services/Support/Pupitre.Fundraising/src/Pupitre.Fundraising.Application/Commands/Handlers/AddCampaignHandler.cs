using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Fundraising.Application.Exceptions;
using Pupitre.Fundraising.Contracts.Commands;
using Pupitre.Fundraising.Domain.Entities;
using Pupitre.Fundraising.Domain.Repositories;

namespace Pupitre.Fundraising.Application.Commands.Handlers;

internal sealed class AddCampaignHandler : ICommandHandler<AddCampaign>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddCampaignHandler(ICampaignRepository campaignRepository,
        IEventProcessor eventProcessor)
    {
        _campaignRepository = campaignRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddCampaign command, CancellationToken cancellationToken = default)
    {
        
        var campaign = await _campaignRepository.GetAsync(command.Id);
        
        if(campaign is not null)
        {
            throw new CampaignAlreadyExistsException(command.Id);
        }

        campaign = Campaign.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _campaignRepository.AddAsync(campaign, cancellationToken);
        await _eventProcessor.ProcessAsync(campaign.Events);
    }
}

