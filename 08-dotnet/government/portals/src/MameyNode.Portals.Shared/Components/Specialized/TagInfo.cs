using MudBlazor;

namespace MameyNode.Portals.Shared.Components.Specialized;

public class TagInfo
{
    public string Label { get; set; } = string.Empty;
    public Color Color { get; set; } = Color.Default;
    public bool Removable { get; set; } = false;
}

