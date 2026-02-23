using Mamey.MessageBrokers;
using Pupitre.Assessments.Application.Commands;
using Pupitre.Assessments.Application.Events;
using Pupitre.Assessments.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.Assessments.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

