using Mamey.MessageBrokers;
using Pupitre.Bookstore.Application.Commands;
using Pupitre.Bookstore.Application.Events;
using Pupitre.Bookstore.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Bookstore.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

