using Microsoft.AspNetCore.Components;

namespace MameyNode.Portals.Shared.Components.DataDisplay;

public class DataTableColumn<TItem>
{
    public string Title { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public RenderFragment<TItem>? Render { get; set; }
    public bool Sortable { get; set; } = true;
}

