# GenderInputPro

A gender selector component for Blazor built on **MudBlazor** that uses the `Mamey.Types.Gender` enum with Display attributes.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<GenderInputPro @bind-Value="_gender" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `Gender?` | `null` | Two-way bindable gender value |
| `Label` | `string` | `"Gender"` | Label text |
| `Required` | `bool` | `false` | Whether field is required |
| `AllowEmpty` | `bool` | `true` | Allow empty selection |

## Usage

```razor
<GenderInputPro 
    Label="Gender"
    Required="true"
    @bind-Value="_gender" />
```

## Supported Values

- Female
- Male




