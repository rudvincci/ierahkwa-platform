using Mamey.MessageBrokers;
using Pupitre.Lessons.Application.Commands;
using Pupitre.Lessons.Application.Events;
using Pupitre.Lessons.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Lessons.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

