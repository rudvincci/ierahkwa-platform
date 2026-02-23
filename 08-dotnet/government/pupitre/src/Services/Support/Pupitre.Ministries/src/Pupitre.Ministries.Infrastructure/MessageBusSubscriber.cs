using Mamey.MessageBrokers;
using Pupitre.Ministries.Application.Commands;
using Pupitre.Ministries.Application.Events;
using Pupitre.Ministries.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Ministries.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

