using Mamey.MessageBrokers;
using Pupitre.Educators.Application.Commands;
using Pupitre.Educators.Application.Events;
using Pupitre.Educators.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Educators.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

