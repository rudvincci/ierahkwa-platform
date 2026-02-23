using Mamey.MessageBrokers;
using Pupitre.Rewards.Application.Commands;
using Pupitre.Rewards.Application.Events;
using Pupitre.Rewards.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Rewards.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

