using Mamey.MessageBrokers;
using Pupitre.Curricula.Application.Commands;
using Pupitre.Curricula.Application.Events;
using Pupitre.Curricula.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Curricula.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

