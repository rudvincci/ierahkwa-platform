# FileUploadPro

An enhanced file upload component for Blazor built on **MudBlazor** with drag-drop, preview, and validation.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<FileUploadPro @bind-Files="_files" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Files` | `IReadOnlyList<IBrowserFile>?` | `null` | Two-way bindable files |
| `FilesChanged` | `EventCallback<IReadOnlyList<IBrowserFile>?>` | - | Event fired when files change |
| `Label` | `string` | `"File Upload"` | Label text |
| `Accept` | `string` | `"*/*"` | Accepted file types |
| `MaxAllowedSize` | `long` | `10MB` | Maximum file size |
| `Multiple` | `bool` | `false` | Allow multiple files |
| `DropZoneHeight` | `string` | `"200px"` | Drop zone height |

## Usage

```razor
<FileUploadPro 
    Label="Upload Files"
    Accept=".pdf,.doc,.docx"
    MaxAllowedSize="5242880"
    Multiple="true"
    @bind-Files="_files" />
```

## Features

- Drag and drop support
- File size validation
- File type validation
- Multiple file support
- File preview with chips




