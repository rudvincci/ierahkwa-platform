using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Fundraising.Application.Exceptions;
using Pupitre.Fundraising.Contracts.Commands;
using Pupitre.Fundraising.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Fundraising.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateCampaignHandler : ICommandHandler<UpdateCampaign>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateCampaignHandler(
        ICampaignRepository campaignRepository,
        IEventProcessor eventProcessor)
    {
        _campaignRepository = campaignRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateCampaign command, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetAsync(command.Id);

        if(campaign is null)
        {
            throw new CampaignNotFoundException(command.Id);
        }

        campaign.Update(command.Name, command.Tags);
        await _campaignRepository.UpdateAsync(campaign);
        await _eventProcessor.ProcessAsync(campaign.Events);
    }
}


