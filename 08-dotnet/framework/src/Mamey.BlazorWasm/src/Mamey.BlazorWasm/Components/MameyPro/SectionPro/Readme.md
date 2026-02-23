# SectionPro

A section divider component for Blazor with title and optional description.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<SectionPro Title="Personal Information" Description="Enter your personal details">
    <ChildContent>
        <NameInputPro @bind-Value="_name" />
    </ChildContent>
</SectionPro>
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` | `null` | Section content |
| `Title` | `string?` | `null` | Section title |
| `Description` | `string?` | `null` | Section description |
| `TitleTypo` | `Typo` | `Typo.h6` | Title typography |
| `ShowDivider` | `bool` | `true` | Show divider below title |

## Usage

```razor
<SectionPro Title="Contact Information" Description="How can we reach you?">
    <ChildContent>
        <EmailInputPro @bind-Value="_email" />
        <PhoneNumberInputPro @bind-Value="_phone" />
    </ChildContent>
</SectionPro>
```




