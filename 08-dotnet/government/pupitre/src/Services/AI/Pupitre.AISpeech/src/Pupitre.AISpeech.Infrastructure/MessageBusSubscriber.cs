using Mamey.MessageBrokers;
using Pupitre.AISpeech.Application.Commands;
using Pupitre.AISpeech.Application.Events;
using Pupitre.AISpeech.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.AISpeech.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

