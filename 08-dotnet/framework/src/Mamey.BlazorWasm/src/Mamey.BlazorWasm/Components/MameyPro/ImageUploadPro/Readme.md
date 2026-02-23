# ImageUploadPro

An image upload component for Blazor with preview, validation, and image-specific features.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<ImageUploadPro @bind-Files="_images" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Files` | `IReadOnlyList<IBrowserFile>?` | `null` | Two-way bindable files |
| `Label` | `string` | `"Image Upload"` | Label text |
| `ImageAccept` | `string` | `"image/*"` | Accepted image types |
| `MaxAllowedSize` | `long` | `5MB` | Maximum file size |
| `Multiple` | `bool` | `false` | Allow multiple images |

## Usage

```razor
<ImageUploadPro 
    Label="Upload Images"
    ImageAccept="image/jpeg,image/png,image/gif"
    MaxAllowedSize="5242880"
    Multiple="true"
    @bind-Files="_images" />
```

## Features

- Image preview with thumbnails
- Image type validation
- File size validation
- Multiple image support
- Grid layout for previews




