using Mamey.MessageBrokers;
using Pupitre.Aftercare.Application.Commands;
using Pupitre.Aftercare.Application.Events;
using Pupitre.Aftercare.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Aftercare.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

