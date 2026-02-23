using Microsoft.AspNetCore.Components;

namespace MameyNode.Portals.Shared.Components.Navigation;

public class TabNavigationItem
{
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public RenderFragment? Content { get; set; }
}

