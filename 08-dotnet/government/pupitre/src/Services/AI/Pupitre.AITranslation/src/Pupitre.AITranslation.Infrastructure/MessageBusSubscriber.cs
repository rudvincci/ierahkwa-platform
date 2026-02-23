using Mamey.MessageBrokers;
using Pupitre.AITranslation.Application.Commands;
using Pupitre.AITranslation.Application.Events;
using Pupitre.AITranslation.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.AITranslation.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

