# DateRangePickerPro

A date range selector component for Blazor that handles start and end dates with validation.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<DateRangePickerPro 
    @bind-StartDate="_startDate"
    @bind-EndDate="_endDate" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `StartDate` | `DateTime?` | `null` | Two-way bindable start date |
| `StartDateChanged` | `EventCallback<DateTime?>` | - | Event fired when start date changes |
| `EndDate` | `DateTime?` | `null` | Two-way bindable end date |
| `EndDateChanged` | `EventCallback<DateTime?>` | - | Event fired when end date changes |
| `StartLabel` | `string` | `"Start Date"` | Start date label |
| `EndLabel` | `string` | `"End Date"` | End date label |
| `Required` | `bool` | `false` | Whether fields are required |

## Usage

```razor
<DateRangePickerPro 
    StartLabel="Check-in Date"
    EndLabel="Check-out Date"
    Required="true"
    @bind-StartDate="_checkIn"
    @bind-EndDate="_checkOut" />
```

## Features

- Start and end date selection
- Automatic validation (end date must be after start date)
- Min/Max date constraints




