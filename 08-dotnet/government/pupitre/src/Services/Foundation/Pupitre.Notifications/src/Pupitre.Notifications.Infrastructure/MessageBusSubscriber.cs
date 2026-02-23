using Mamey.MessageBrokers;
using Pupitre.Notifications.Application.Commands;
using Pupitre.Notifications.Application.Events;
using Pupitre.Notifications.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Notifications.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

