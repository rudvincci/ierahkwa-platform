using Mamey.MessageBrokers;
using Pupitre.AITutors.Application.Commands;
using Pupitre.AITutors.Application.Events;
using Pupitre.AITutors.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.AITutors.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

