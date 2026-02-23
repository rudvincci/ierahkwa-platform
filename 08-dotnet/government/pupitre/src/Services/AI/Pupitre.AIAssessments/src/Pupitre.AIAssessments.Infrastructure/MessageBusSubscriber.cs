using Mamey.MessageBrokers;
using Pupitre.AIAssessments.Application.Commands;
using Pupitre.AIAssessments.Application.Events;
using Pupitre.AIAssessments.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;
namespace Pupitre.AIAssessments.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        //bus.SubscribeCommand<>();
        //bus.SubscribeEvent<>();
        return bus;
    }
}

