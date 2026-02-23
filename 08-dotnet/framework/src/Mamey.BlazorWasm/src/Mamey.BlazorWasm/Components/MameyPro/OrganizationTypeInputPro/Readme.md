# OrganizationTypeInputPro

An organization type selector component for Blazor built on **MudBlazor** that uses the `Mamey.Types.OrganizationType` enum with Display attributes.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<OrganizationTypeInputPro @bind-Value="_type" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `OrganizationType?` | `null` | Two-way bindable organization type value |
| `Label` | `string` | `"Organization Type"` | Label text |
| `Required` | `bool` | `false` | Whether field is required |

## Usage

```razor
<OrganizationTypeInputPro 
    Label="Organization Type"
    Required="true"
    @bind-Value="_type" />
```

## Supported Values

Includes all organization types from the enum (e.g., INKGIncorporatedCompany, SAGCompany, SoleProprietor, LLC, etc.) with Display attribute names shown in the dropdown.




