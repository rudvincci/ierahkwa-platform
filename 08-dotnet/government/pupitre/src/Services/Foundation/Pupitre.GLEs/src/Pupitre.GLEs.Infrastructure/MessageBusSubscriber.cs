using Mamey.MessageBrokers;
using Pupitre.GLEs.Application.Commands;
using Pupitre.GLEs.Application.Events;
using Pupitre.GLEs.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.GLEs.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

