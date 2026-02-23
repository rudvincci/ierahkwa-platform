# IndustryTypeInputPro

An industry type selector component for Blazor built on **MudBlazor** that uses the `Mamey.Types.IndustryType` enum with Display attributes.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<IndustryTypeInputPro @bind-Value="_type" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `IndustryType?` | `null` | Two-way bindable industry type value |
| `Label` | `string` | `"Industry Type"` | Label text |
| `Required` | `bool` | `false` | Whether field is required |

## Usage

```razor
<IndustryTypeInputPro 
    Label="Industry Type"
    Required="true"
    @bind-Value="_type" />
```

## Supported Values

Includes all industry types from the enum (e.g., Holding, Hotel-Motel, Restaurants, Construction, etc.) with Display attribute names shown in the dropdown.




