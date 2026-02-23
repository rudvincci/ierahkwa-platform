# LinkInputPro

A link input component for Blazor built on **MudBlazor** that handles URL, Href, and Method fields using the `Mamey.Types.Link` class.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<LinkInputPro @bind-Value="_link" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `Link?` | `null` | Two-way bindable link value |
| `Label` | `string` | `"Link"` | Label text |
| `HelperText` | `string` | `"Enter link information"` | Helper text |
| `Required` | `bool` | `false` | Whether URL is required |

## Usage

```razor
<LinkInputPro 
    Label="Link"
    Required="true"
    @bind-Value="_link" />
```

## Fields

- **URL**: Full URL (e.g., https://example.com)
- **Href**: Relative path (e.g., /path/to/resource)
- **Method**: HTTP method (e.g., GET, POST)




