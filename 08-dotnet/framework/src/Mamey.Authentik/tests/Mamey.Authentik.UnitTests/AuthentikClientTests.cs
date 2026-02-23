using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Mamey.Authentik.Services;
using Xunit;

namespace Mamey.Authentik.UnitTests;

public class AuthentikClientTests
{
    [Fact]
    public void Constructor_WithAllServices_CreatesClient()
    {
        // Arrange
        var admin = Mock.Of<IAuthentikAdminService>();
        var core = Mock.Of<IAuthentikCoreService>();
        var oauth2 = Mock.Of<IAuthentikOAuth2Service>();
        var flows = Mock.Of<IAuthentikFlowsService>();
        var policies = Mock.Of<IAuthentikPoliciesService>();
        var providers = Mock.Of<IAuthentikProvidersService>();
        var stages = Mock.Of<IAuthentikStagesService>();
        var sources = Mock.Of<IAuthentikSourcesService>();
        var events = Mock.Of<IAuthentikEventsService>();
        var authenticators = Mock.Of<IAuthentikAuthenticatorsService>();
        var crypto = Mock.Of<IAuthentikCryptoService>();
        var propertyMappings = Mock.Of<IAuthentikPropertyMappingsService>();
        var rac = Mock.Of<IAuthentikRacService>();
        var rbac = Mock.Of<IAuthentikRbacService>();
        var tenants = Mock.Of<IAuthentikTenantsService>();
        var tasks = Mock.Of<IAuthentikTasksService>();
        var outposts = Mock.Of<IAuthentikOutpostsService>();
        var endpoints = Mock.Of<IAuthentikEndpointsService>();
        var enterprise = Mock.Of<IAuthentikEnterpriseService>();
        var managed = Mock.Of<IAuthentikManagedService>();
        var reports = Mock.Of<IAuthentikReportsService>();
        var root = Mock.Of<IAuthentikRootService>();
        var ssf = Mock.Of<IAuthentikSsfService>();

        // Act
        var client = new AuthentikClient(
            admin, core, oauth2, flows, policies, providers, stages, sources, events,
            authenticators, crypto, propertyMappings, rac, rbac, tenants, tasks, outposts,
            endpoints, enterprise, managed, reports, root, ssf);

        // Assert
        client.Should().NotBeNull();
        client.Admin.Should().Be(admin);
        client.Core.Should().Be(core);
        client.OAuth2.Should().Be(oauth2);
        client.Flows.Should().Be(flows);
        client.Policies.Should().Be(policies);
        client.Providers.Should().Be(providers);
        client.Stages.Should().Be(stages);
        client.Sources.Should().Be(sources);
        client.Events.Should().Be(events);
        client.Authenticators.Should().Be(authenticators);
        client.Crypto.Should().Be(crypto);
        client.PropertyMappings.Should().Be(propertyMappings);
        client.Rac.Should().Be(rac);
        client.Rbac.Should().Be(rbac);
        client.Tenants.Should().Be(tenants);
        client.Tasks.Should().Be(tasks);
        client.Outposts.Should().Be(outposts);
        client.Endpoints.Should().Be(endpoints);
        client.Enterprise.Should().Be(enterprise);
        client.Managed.Should().Be(managed);
        client.Reports.Should().Be(reports);
        client.Root.Should().Be(root);
        client.Ssf.Should().Be(ssf);
    }
}
