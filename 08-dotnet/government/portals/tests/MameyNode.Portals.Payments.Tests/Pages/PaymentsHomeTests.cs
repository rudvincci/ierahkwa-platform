using Bunit;
using FluentAssertions;
using MameyNode.Portals.Payments.Pages;
using Xunit;

namespace MameyNode.Portals.Payments.Tests.Pages;

public class PaymentsHomeTests : TestContext
{
    [Fact]
    public void PaymentsHome_ShouldRender()
    {
        // Arrange & Act
        var component = RenderComponent<PaymentsHome>();

        // Assert
        component.Should().NotBeNull();
    }

    [Fact]
    public void PaymentsHome_ShouldDisplayTitle()
    {
        // Arrange & Act
        var component = RenderComponent<PaymentsHome>();

        // Assert
        component.Markup.Should().Contain("Payments Portal");
    }

    [Fact]
    public void PaymentsHome_ShouldDisplayStatCards()
    {
        // Arrange & Act
        var component = RenderComponent<PaymentsHome>();

        // Assert
        component.Markup.Should().Contain("Total Payments");
        component.Markup.Should().Contain("Active Subscriptions");
    }

    [Fact]
    public void PaymentsHome_ShouldDisplayNavigationCards()
    {
        // Arrange & Act
        var component = RenderComponent<PaymentsHome>();

        // Assert
        component.Markup.Should().Contain("P2P Payments");
        component.Markup.Should().Contain("Merchant Payments");
    }
}

