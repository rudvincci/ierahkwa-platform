# MaritalStatusInputPro

A marital status selector component for Blazor built on **MudBlazor** that uses the `Mamey.Types.MaritalStatus` enum.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<MaritalStatusInputPro @bind-Value="_status" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `MaritalStatus?` | `null` | Two-way bindable marital status value |
| `Label` | `string` | `"Marital Status"` | Label text |
| `Required` | `bool` | `false` | Whether field is required |

## Usage

```razor
<MaritalStatusInputPro 
    Label="Marital Status"
    Required="true"
    @bind-Value="_status" />
```

## Supported Values

- Single
- Married
- Divorced
- Widowed




