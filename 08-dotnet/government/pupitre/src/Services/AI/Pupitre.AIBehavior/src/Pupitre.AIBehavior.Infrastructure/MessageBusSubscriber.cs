using Mamey.MessageBrokers;
using Pupitre.AIBehavior.Application.Commands;
using Pupitre.AIBehavior.Application.Events;
using Pupitre.AIBehavior.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.AIBehavior.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

