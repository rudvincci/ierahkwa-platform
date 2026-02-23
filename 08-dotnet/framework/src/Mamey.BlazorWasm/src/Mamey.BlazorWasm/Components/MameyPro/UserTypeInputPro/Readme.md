# UserTypeInputPro

A user type selector component for Blazor built on **MudBlazor** that uses the `Mamey.Types.UserType` enum.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<UserTypeInputPro @bind-Value="_type" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `UserType?` | `null` | Two-way bindable user type value |
| `Label` | `string` | `"User Type"` | Label text |
| `Required` | `bool` | `false` | Whether field is required |

## Usage

```razor
<UserTypeInputPro 
    Label="User Type"
    @bind-Value="_type" />
```

## Supported Values

- Applicant
- User
- System
- Employee
- Administrator
- Daemon




