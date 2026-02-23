# FormPro

A form wrapper component for Blazor that integrates with `EditForm` and provides validation state management.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<FormPro Model="@_model" OnValidSubmit="HandleSubmit">
    <ChildContent>
        <EmailInputPro @bind-Value="_model.Email" />
        <MudButton ButtonType="ButtonType.Submit">Submit</MudButton>
    </ChildContent>
</FormPro>
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Model` | `object?` | `null` | Form model object |
| `OnValidSubmit` | `EventCallback<EditContext>` | - | Called when form is valid |
| `OnInvalidSubmit` | `EventCallback<EditContext>` | - | Called when form is invalid |
| `ChildContent` | `RenderFragment?` | `null` | Form content |
| `ShowValidationSummary` | `bool` | `false` | Show validation summary |
| `Spacing` | `int` | `3` | Spacing between form fields |

## Usage

```razor
<FormPro Model="@_user" OnValidSubmit="SaveUser" ShowValidationSummary="true">
    <ChildContent>
        <NameInputPro @bind-Value="_user.Name" />
        <EmailInputPro @bind-Value="_user.Email" />
        <MudButton ButtonType="ButtonType.Submit">Save</MudButton>
    </ChildContent>
</FormPro>
```




