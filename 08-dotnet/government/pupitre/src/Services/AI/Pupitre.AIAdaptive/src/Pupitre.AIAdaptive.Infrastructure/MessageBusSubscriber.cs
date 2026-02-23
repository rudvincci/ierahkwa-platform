using Mamey.MessageBrokers;
using Pupitre.AIAdaptive.Application.Commands;
using Pupitre.AIAdaptive.Application.Events;
using Pupitre.AIAdaptive.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.AIAdaptive.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

