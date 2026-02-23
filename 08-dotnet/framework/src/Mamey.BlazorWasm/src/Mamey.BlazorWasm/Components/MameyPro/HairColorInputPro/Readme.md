# HairColorInputPro

A hair color selector component for Blazor built on **MudBlazor** that uses the `Mamey.Types.HairColor` enum.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<HairColorInputPro @bind-Value="_color" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `HairColor?` | `null` | Two-way bindable hair color value |
| `Label` | `string` | `"Hair Color"` | Label text |
| `Required` | `bool` | `false` | Whether field is required |

## Usage

```razor
<HairColorInputPro 
    Label="Hair Color"
    @bind-Value="_color" />
```

## Supported Values

- Bald
- Black
- Blonde
- Brown
- Gray
- RedAuburn
- Sandy
- White
- Unknown




