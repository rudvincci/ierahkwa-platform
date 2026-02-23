# EyeColorInputPro

An eye color selector component for Blazor built on **MudBlazor** that uses the `Mamey.Types.EyeColor` enum.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<EyeColorInputPro @bind-Value="_color" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `EyeColor?` | `null` | Two-way bindable eye color value |
| `Label` | `string` | `"Eye Color"` | Label text |
| `Required` | `bool` | `false` | Whether field is required |

## Usage

```razor
<EyeColorInputPro 
    Label="Eye Color"
    @bind-Value="_color" />
```

## Supported Values

- Blue
- Brown
- Black
- Hazel
- Green
- Gray
- Pink
- Maroon
- Dichromatic




