using Mamey.MessageBrokers;
using Pupitre.AIVision.Application.Commands;
using Pupitre.AIVision.Application.Events;
using Pupitre.AIVision.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.AIVision.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

