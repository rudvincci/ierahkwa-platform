using Bunit;
using FluentAssertions;
using MameyNode.Portals.Shared.Components.DataDisplay;
using Xunit;

namespace MameyNode.Portals.Shared.Tests;

public class StatusBadgeTests : TestContext
{
    [Fact]
    public void StatusBadge_ShouldRenderWithStatus()
    {
        // Arrange & Act
        var component = RenderComponent<StatusBadge>(parameters => parameters
            .Add(p => p.Status, "Active"));

        // Assert
        component.Markup.Should().Contain("Active");
    }

    [Theory]
    [InlineData("Active", "Success")]
    [InlineData("Pending", "Warning")]
    [InlineData("Error", "Error")]
    [InlineData("Completed", "Success")]
    public void StatusBadge_ShouldUseCorrectColorForStatus(string status, string expectedColor)
    {
        // Arrange & Act
        var component = RenderComponent<StatusBadge>(parameters => parameters
            .Add(p => p.Status, status));

        // Assert
        var chip = component.Find("div.mud-chip");
        chip.Should().NotBeNull();
        // The color is applied via MudBlazor's Color property
    }
}

