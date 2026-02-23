using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Fundraising.Application.Events.Rejected;
using Pupitre.Fundraising.Application.Exceptions;
using System;
using Pupitre.Fundraising.Domain.Exceptions;
using Pupitre.Fundraising.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Fundraising.Contracts.Commands;

namespace Pupitre.Fundraising.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            CampaignAlreadyExistsException ex => message switch
            {
                AddCampaign cmd => new AddCampaignRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            CampaignNotFoundException ex => message switch
            {
                UpdateCampaign cmd => new UpdateCampaignRejected(ex.CampaignId, ex.Message, ex.Code),
                DeleteCampaign cmd => new DeleteCampaignRejected(ex.CampaignId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

