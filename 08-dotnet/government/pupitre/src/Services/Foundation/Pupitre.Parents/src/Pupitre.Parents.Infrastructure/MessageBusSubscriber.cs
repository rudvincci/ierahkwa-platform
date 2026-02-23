using Mamey.MessageBrokers;
using Pupitre.Parents.Application.Commands;
using Pupitre.Parents.Application.Events;
using Pupitre.Parents.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Parents.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

