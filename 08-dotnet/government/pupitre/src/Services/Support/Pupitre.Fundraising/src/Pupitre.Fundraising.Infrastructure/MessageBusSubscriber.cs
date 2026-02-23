using Mamey.MessageBrokers;
using Pupitre.Fundraising.Application.Commands;
using Pupitre.Fundraising.Application.Events;
using Pupitre.Fundraising.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Fundraising.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

