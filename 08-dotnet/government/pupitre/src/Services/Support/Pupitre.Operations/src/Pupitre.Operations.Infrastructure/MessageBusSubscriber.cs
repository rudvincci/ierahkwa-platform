using Mamey.MessageBrokers;
using Pupitre.Operations.Application.Commands;
using Pupitre.Operations.Application.Events;
using Pupitre.Operations.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Operations.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

