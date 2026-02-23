# CardPro

An enhanced card component for Blazor built on **MudBlazor** with header, content, and actions slots.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<CardPro HeaderText="My Card">
    <ChildContent>
        <p>Card content goes here</p>
    </ChildContent>
    <Actions>
        <MudButton>Action</MudButton>
    </Actions>
</CardPro>
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Header` | `RenderFragment?` | `null` | Custom header content |
| `HeaderActions` | `RenderFragment?` | `null` | Actions in header area |
| `ChildContent` | `RenderFragment?` | `null` | Main card content |
| `Actions` | `RenderFragment?` | `null` | Actions at bottom |
| `HeaderText` | `string?` | `null` | Header text (if not using Header slot) |
| `ShowHeader` | `bool` | `true` | Show header section |
| `ShowActions` | `bool` | `true` | Show actions section |
| `Elevation` | `int` | `1` | Paper elevation |

## Usage

```razor
<CardPro HeaderText="User Profile" Elevation="2">
    <ChildContent>
        <p>User information</p>
    </ChildContent>
    <Actions>
        <MudButton Variant="Variant.Filled">Save</MudButton>
    </Actions>
</CardPro>
```




