using Mamey.MessageBrokers;
using Pupitre.Accessibility.Application.Commands;
using Pupitre.Accessibility.Application.Events;
using Pupitre.Accessibility.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Accessibility.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

