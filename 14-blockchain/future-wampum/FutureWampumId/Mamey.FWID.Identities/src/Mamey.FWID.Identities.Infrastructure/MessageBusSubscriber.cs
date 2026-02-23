using Mamey.MessageBrokers;
using Mamey.FWID.Identities.Application.Commands;
using Mamey.FWID.Identities.Application.Commands.Integration.Identities;
using Mamey.FWID.Identities.Application.Events;
using Mamey.FWID.Identities.Application.Events.Integration.AccessControls;
using Mamey.FWID.Identities.Application.Events.Integration.Credentials;
using Mamey.FWID.Identities.Application.Events.Integration.DIDs;
using Mamey.FWID.Identities.Application.Events.Integration.ZKPs;
using Mamey.FWID.Identities.Application.Events.Rejected;
using Mamey.MessageBrokers.CQRS;

namespace Mamey.FWID.Identities.Infrastructure;

public static class MessageBusSubscriber
{
    public static IBusSubscriber AddSubscriptions(this IBusSubscriber bus)
    {
        // Subscribe to integration commands from other services
        bus.SubscribeCommand<CreateIdentityIntegrationCommand>();
        bus.SubscribeCommand<VerifyIdentityIntegrationCommand>();
        
        // Subscribe to integration events from other services
        bus.SubscribeEvent<DIDCreatedIntegrationEvent>(); // From DIDs service
        bus.SubscribeEvent<CredentialIssuedIntegrationEvent>(); // From Credentials service
        bus.SubscribeEvent<ZKPProofGeneratedIntegrationEvent>(); // From ZKPs service
        bus.SubscribeEvent<ZoneAccessGrantedIntegrationEvent>(); // From AccessControls service
        
        return bus;
    }
}

