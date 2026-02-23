using Mamey.MessageBrokers;
using Pupitre.AIRecommendations.Application.Commands;
using Pupitre.AIRecommendations.Application.Events;
using Pupitre.AIRecommendations.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.AIRecommendations.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

