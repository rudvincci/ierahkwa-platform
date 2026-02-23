# EducationLevelInputPro

An education level selector component for Blazor built on **MudBlazor** that uses the `Mamey.Types.EducationLevel` enum.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<EducationLevelInputPro @bind-Value="_level" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `EducationLevel?` | `null` | Two-way bindable education level value |
| `Label` | `string` | `"Education Level"` | Label text |
| `Required` | `bool` | `false` | Whether field is required |

## Usage

```razor
<EducationLevelInputPro 
    Label="Education Level"
    @bind-Value="_level" />
```

## Supported Values

- HighSchool
- Bachelor
- Master
- Doctorate
- Other




