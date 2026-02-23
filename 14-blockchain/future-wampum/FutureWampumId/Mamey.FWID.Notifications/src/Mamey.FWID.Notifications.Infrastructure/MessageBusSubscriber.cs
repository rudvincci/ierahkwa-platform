using Mamey.FWID.Notifications.Application.Commands.Integration.Notifications;
using Mamey.FWID.Notifications.Application.Events.Integration.AccessControls;
using Mamey.FWID.Notifications.Application.Events.Integration.Credentials;
using Mamey.FWID.Notifications.Application.Events.Integration.DIDs;
using Mamey.FWID.Notifications.Application.Events.Integration.Identities;
using Mamey.FWID.Notifications.Application.Events.Integration.ZKPs;
using Mamey.MessageBrokers;
using Mamey.MessageBrokers.CQRS;

namespace Mamey.FWID.Notifications.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        // Subscribe to integration commands from other services
        bus.SubscribeCommand<SendNotificationIntegrationCommand>();

        // Subscribe to integration events from all 6 FWID services
        bus.SubscribeEvent<IdentityCreatedIntegrationEvent>(); // From Identities service
        bus.SubscribeEvent<DIDCreatedIntegrationEvent>(); // From DIDs service
        bus.SubscribeEvent<ZKPProofGeneratedIntegrationEvent>(); // From ZKPs service
        bus.SubscribeEvent<CredentialIssuedIntegrationEvent>(); // From Credentials service
        bus.SubscribeEvent<ZoneAccessGrantedIntegrationEvent>(); // From AccessControls service

        return bus;
    }
}







