using Mamey.MessageBrokers.CQRS;
using Mamey.MessageBrokers;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.DIDs.Domain.Events;
using Mamey.FWID.Credentials.Domain.Events;
using Mamey.FWID.AccessControls.Domain.Events;
using Mamey.FWID.ZKPs.Domain.Events;

namespace Mamey.FWID.Sagas.Core;

public static class RabbitMQSubscriptions
{
    public static IBusSubscriber AddRabbitMQEventSubscriptions(this IBusSubscriber subscriber)
    {
        subscriber
            // Identity events
            .SubscribeEvent<IdentityCreated>()
            // DID events
            .SubscribeEvent<DIDCreated>()
            // Credential events
            .SubscribeEvent<CredentialIssued>()
            // Access Control events
            .SubscribeEvent<ZoneAccessGranted>()
            // ZKP events
            .SubscribeEvent<ZKPProofGenerated>();
        return subscriber;
    }

    public static IBusSubscriber AddRabbitMQCommandSubscriptions(this IBusSubscriber subscriber)
    {
        // Commands are typically sent, not subscribed to
        return subscriber;
    }
}



