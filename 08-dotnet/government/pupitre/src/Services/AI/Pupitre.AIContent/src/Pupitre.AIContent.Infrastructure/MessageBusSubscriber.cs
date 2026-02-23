using Mamey.MessageBrokers;
using Pupitre.AIContent.Application.Commands;
using Pupitre.AIContent.Application.Events;
using Pupitre.AIContent.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.AIContent.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

