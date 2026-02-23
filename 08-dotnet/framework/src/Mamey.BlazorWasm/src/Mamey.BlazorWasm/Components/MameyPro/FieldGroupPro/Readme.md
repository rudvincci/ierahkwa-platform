# FieldGroupPro

A field grouping component for Blazor that groups related fields with shared validation state.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<FieldGroupPro Label="Address" HelperText="Enter your address">
    <ChildContent>
        <MudItem xs="12"><MudTextField Label="Street" /></MudItem>
        <MudItem xs="6"><MudTextField Label="City" /></MudItem>
        <MudItem xs="6"><MudTextField Label="State" /></MudItem>
    </ChildContent>
</FieldGroupPro>
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` | `null` | Grouped fields |
| `Label` | `string?` | `null` | Group label |
| `HelperText` | `string?` | `null` | Helper text |
| `Spacing` | `int` | `2` | Spacing between fields |

## Usage

```razor
<FieldGroupPro Label="Name" HelperText="Enter your full name">
    <ChildContent>
        <MudItem xs="6"><MudTextField Label="First Name" /></MudItem>
        <MudItem xs="6"><MudTextField Label="Last Name" /></MudItem>
    </ChildContent>
</FieldGroupPro>
```




