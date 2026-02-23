using FluentAssertions;
using MameyNode.Portals.Payments;
using Xunit;

namespace MameyNode.Portals.Payments.Tests;

public class PaymentsRouteServiceTests
{
    [Fact]
    public async Task InitializeAsync_ShouldRegisterAllRoutes()
    {
        // Arrange
        var service = new PaymentsRouteService();
        
        // Act
        await service.InitializeAsync();
        
        // Assert
        service.Routes.Should().HaveCount(1);
        service.Routes[0].Page.Should().Be("/payments");
        service.Routes[0].Title.Should().Be("Payments");
        service.Routes[0].Icon.Should().Be("fas fa-credit-card");
        service.Routes[0].AuthenticationRequired.Should().BeTrue();
        service.Routes[0].RequiredRoles.Should().Contain("User");
        service.Routes[0].RequiredRoles.Should().Contain("Payments");
        service.Routes[0].ChildRoutes.Should().HaveCount(11);
    }

    [Fact]
    public async Task InitializeAsync_ShouldRegisterChildRoutes()
    {
        // Arrange
        var service = new PaymentsRouteService();
        
        // Act
        await service.InitializeAsync();
        
        // Assert
        var childRoutes = service.Routes[0].ChildRoutes;
        childRoutes.Should().Contain(r => r.Page == "/payments/p2p" && r.Title == "P2P Payments");
        childRoutes.Should().Contain(r => r.Page == "/payments/merchant" && r.Title == "Merchant Payments");
        childRoutes.Should().Contain(r => r.Page == "/payments/gateway" && r.Title == "Payment Gateway");
    }

    [Fact]
    public async Task InitializeAsync_ShouldInvokeRoutesChangedEvent()
    {
        // Arrange
        var service = new PaymentsRouteService();
        var eventInvoked = false;
        service.RoutesChanged += (routes) => eventInvoked = true;
        
        // Act
        await service.InitializeAsync();
        
        // Assert
        eventInvoked.Should().BeTrue();
    }
}

