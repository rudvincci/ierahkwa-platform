# EmploymentStatusInputPro

An employment status selector component for Blazor built on **MudBlazor** that uses the `Mamey.Types.EmploymentStatus` enum.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<EmploymentStatusInputPro @bind-Value="_status" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `EmploymentStatus?` | `null` | Two-way bindable employment status value |
| `Label` | `string` | `"Employment Status"` | Label text |
| `Required` | `bool` | `false` | Whether field is required |

## Usage

```razor
<EmploymentStatusInputPro 
    Label="Employment Status"
    @bind-Value="_status" />
```

## Supported Values

- Employed
- SelfEmployed
- Unemployed
- Retired
- Student




