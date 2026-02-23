using Mamey.MessageBrokers;
using Pupitre.Progress.Application.Commands;
using Pupitre.Progress.Application.Events;
using Pupitre.Progress.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Progress.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

