using Mamey.MessageBrokers;
using Mamey.ServiceName.Application.Commands;
using Mamey.ServiceName.Application.Events;
using Mamey.ServiceName.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Mamey.ServiceName.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

