using Mamey.MessageBrokers;
using Pupitre.Users.Application.Commands;
using Pupitre.Users.Application.Events;
using Pupitre.Users.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Users.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

