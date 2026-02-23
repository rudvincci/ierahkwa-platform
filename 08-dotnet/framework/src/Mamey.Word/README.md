# Mamey.Word

The Mamey.Word library provides Microsoft Word document generation and processing capabilities for the Mamey framework.

## Overview

Mamey.Word provides comprehensive Word document processing capabilities for the Mamey framework. It enables generation and processing of Word documents programmatically.

### Key Features

- **Document Generation**: Create Word documents programmatically
- **Template Processing**: Process Word templates with placeholders
- **Formatting**: Text and document formatting
- **Conversion**: Convert to PDF or other formats
- **Merge Fields**: Mail merge capabilities

## Installation

```bash
dotnet add package Mamey.Word
```

## Usage

### Basic Setup

```csharp
using Mamey.Word;
using Mamey;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddWord();
```

### Generate Word Document

```csharp
public class DocumentService
{
    private readonly IWordService _wordService;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(IWordService wordService, ILogger<DocumentService> logger)
    {
        _wordService = wordService;
        _logger = logger;
    }

    public async Task<byte[]> GenerateDocumentAsync(DocumentData data)
    {
        try
        {
            var document = await _wordService.GenerateDocumentAsync(data);
            _logger.LogInformation("Successfully generated Word document");
            return document;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate Word document");
            throw;
        }
    }
}
```

### Process Template

```csharp
public class TemplateService
{
    private readonly IWordService _wordService;

    public async Task<byte[]> GenerateFromTemplateAsync(string templatePath, object data)
    {
        return await _wordService.ProcessTemplateAsync(templatePath, data);
    }
}
```

## Word Features

- **Document Creation**: Create new Word documents
- **Template Processing**: Process Word templates with placeholder replacement
- **Text Formatting**: Format text with styles, fonts, and colors
- **Paragraph Formatting**: Format paragraphs with alignment and spacing
- **Tables**: Create and format tables
- **Headers and Footers**: Add headers and footers to documents
- **Mail Merge**: Support for mail merge operations

## Related Libraries

- **Mamey.Excel** - Excel document processing
- **Mamey.Templates** - Template management
- **Mamey.Adobe** - PDF conversion

## Additional Resources

- [Mamey Framework Documentation](../../documentation/)
- [Mamey.Word Memory Documentation](../../.skmemory/v1/memory/public/mid-term/libraries/document-processing/mamey-word.md)

