# DateTimePickerPro

A date and time picker component for Blazor that combines date and time selection.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<DateTimePickerPro @bind-Value="_dateTime" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `DateTime?` | `null` | Two-way bindable date/time value |
| `Label` | `string` | `"Date & Time"` | Label text |
| `HelperText` | `string` | `"Select date and time"` | Helper text |
| `Required` | `bool` | `false` | Whether field is required |

## Usage

```razor
<DateTimePickerPro 
    Label="Appointment Date & Time"
    Required="true"
    @bind-Value="_appointmentDateTime" />
```

## Features

- Combined date and time selection
- Separate date and time pickers
- Automatic value combination




