# FrequencyTypeInputPro

A frequency type selector component for Blazor built on **MudBlazor** that uses the `Mamey.Types.FrequencyType` enum.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<FrequencyTypeInputPro @bind-Value="_frequency" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `FrequencyType?` | `null` | Two-way bindable frequency value |
| `Label` | `string` | `"Frequency"` | Label text |
| `Required` | `bool` | `false` | Whether field is required |

## Usage

```razor
<FrequencyTypeInputPro 
    Label="Frequency"
    @bind-Value="_frequency" />
```

## Supported Values

- Daily
- Weekly
- BiWeekly
- BiMonthly
- Monthy
- Quarterly
- Yearly




