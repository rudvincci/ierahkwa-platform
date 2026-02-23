// Components/Foundation/MameyInputBase.cs
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Mamey.BlazorWasm.Components.Foundation;

public abstract class MameyInputBase<T> : ComponentBase
{
    [Parameter] public string Label { get; set; } = "";
    [Parameter] public string HelperText { get; set; } = "";
    [Parameter] public string Placeholder { get; set; } = "";
    [Parameter] public bool Required { get; set; } = false;
    [Parameter] public bool Disabled { get; set; } = false;
    [Parameter] public Variant Variant { get; set; } = Variant.Text;
    [Parameter] public Adornment Adornment { get; set; } = Adornment.None;
    [Parameter] public string? AdornmentIcon { get; set; }
    [Parameter] public string Class { get; set; } = "";
    [Parameter] public string Style { get; set; } = "";

    [Parameter] public T? Value { get; set; }
    [Parameter] public EventCallback<T?> ValueChanged { get; set; }

    protected Task Emit(T? v) => ValueChanged.InvokeAsync(v);
}
