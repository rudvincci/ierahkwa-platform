# DocumentUploadPro

A document-specific upload component for Blazor with type validation for document files.

## Quick start

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<DocumentUploadPro @bind-Files="_documents" />
```

## API Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Files` | `IReadOnlyList<IBrowserFile>?` | `null` | Two-way bindable files |
| `Label` | `string` | `"Document Upload"` | Label text |
| `DocumentAccept` | `string` | `".pdf,.doc,.docx,.txt,.rtf"` | Accepted document types |
| `MaxAllowedSize` | `long` | `10MB` | Maximum file size |
| `Multiple` | `bool` | `false` | Allow multiple files |

## Usage

```razor
<DocumentUploadPro 
    Label="Upload Documents"
    DocumentAccept=".pdf,.doc,.docx"
    MaxAllowedSize="10485760"
    Multiple="true"
    @bind-Files="_documents" />
```

## Features

- Document type validation
- File size validation
- Multiple document support
- File preview with chips




