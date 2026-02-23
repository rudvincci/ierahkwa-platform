using Mamey.MessageBrokers;
using Pupitre.Compliance.Application.Commands;
using Pupitre.Compliance.Application.Events;
using Pupitre.Compliance.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Compliance.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

