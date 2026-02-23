using Mamey.MessageBrokers;
using Pupitre.AISafety.Application.Commands;
using Pupitre.AISafety.Application.Events;
using Pupitre.AISafety.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.AISafety.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

