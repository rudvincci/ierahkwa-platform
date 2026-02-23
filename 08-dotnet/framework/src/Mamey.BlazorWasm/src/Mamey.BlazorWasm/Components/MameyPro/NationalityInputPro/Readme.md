# NationalityInputPro

A nationality selector component for Blazor built on **MudBlazor** that uses the `Mamey.Types.Nationality` value object. Supports the allowed nationality codes (PL, DE, FR, ES, GB).

---

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<NationalityInputPro @bind-Value="_nationality" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `Nationality?` | `null` | Two-way bindable nationality value |
| `ValueChanged` | `EventCallback<Nationality?>` | - | Event fired when nationality changes |
| `Label` | `string` | `"Nationality"` | Label text |
| `HelperText` | `string` | `"Select your nationality"` | Helper text |
| `Required` | `bool` | `false` | Whether field is required |
| `Disabled` | `bool` | `false` | Whether input is disabled |
| `AllowEmpty` | `bool` | `true` | Allow empty selection |

## Usage

```razor
<NationalityInputPro 
    Label="Nationality"
    Required="true"
    @bind-Value="_nationality" />
```

## Supported Nationalities

- PL - Poland
- DE - Germany
- FR - France
- ES - Spain
- GB - United Kingdom




