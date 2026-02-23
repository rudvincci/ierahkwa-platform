# DatePickerPro

A date picker component for Blazor built on **MudBlazor** with validation and formatting.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<DatePickerPro @bind-Value="_date" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `DateTime?` | `null` | Two-way bindable date value |
| `Label` | `string` | `"Date"` | Label text |
| `HelperText` | `string` | `"Select a date"` | Helper text |
| `Required` | `bool` | `false` | Whether field is required |
| `MinDate` | `DateTime?` | `null` | Minimum allowed date |
| `MaxDate` | `DateTime?` | `null` | Maximum allowed date |

## Usage

```razor
<DatePickerPro 
    Label="Birth Date"
    MinDate="@DateTime.Today.AddYears(-100)"
    MaxDate="@DateTime.Today"
    Required="true"
    @bind-Value="_birthDate" />
```

## Features

- Date range validation
- Required field support
- MudBlazor date picker integration




