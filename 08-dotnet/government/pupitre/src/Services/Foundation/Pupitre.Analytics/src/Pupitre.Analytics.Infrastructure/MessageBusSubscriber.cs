using Mamey.MessageBrokers;
using Pupitre.Analytics.Application.Commands;
using Pupitre.Analytics.Application.Events;
using Pupitre.Analytics.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Analytics.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

