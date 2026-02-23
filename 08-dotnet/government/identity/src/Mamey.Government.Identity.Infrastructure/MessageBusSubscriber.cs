using Mamey.MessageBrokers;
using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Application.Events;
using Mamey.Government.Identity.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Mamey.Government.Identity.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

