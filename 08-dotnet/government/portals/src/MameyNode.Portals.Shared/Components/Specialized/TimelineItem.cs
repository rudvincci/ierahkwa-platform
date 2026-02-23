using MudBlazor;

namespace MameyNode.Portals.Shared.Components.Specialized;

public class TimelineItem
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Time { get; set; }
    public string? Icon { get; set; }
    public Color Color { get; set; } = Color.Primary;
    public Size Size { get; set; } = Size.Medium;
    public Variant Variant { get; set; } = Variant.Filled;
}

