# TimePickerPro

A time picker component for Blazor built on **MudBlazor** with validation.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<TimePickerPro @bind-Value="_time" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `TimeSpan?` | `null` | Two-way bindable time value |
| `Label` | `string` | `"Time"` | Label text |
| `HelperText` | `string` | `"Select a time"` | Helper text |
| `Required` | `bool` | `false` | Whether field is required |

## Usage

```razor
<TimePickerPro 
    Label="Appointment Time"
    Required="true"
    @bind-Value="_appointmentTime" />
```

## Features

- Time selection with MudBlazor time picker
- Required field support
- TimeSpan value binding




