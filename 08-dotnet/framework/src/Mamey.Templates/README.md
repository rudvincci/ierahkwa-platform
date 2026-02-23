# Mamey.Templates

The Mamey.Templates library provides template-based document generation capabilities for the Mamey framework.

## Overview

Mamey.Templates provides comprehensive template management and document generation capabilities for the Mamey framework. It enables creation of documents from templates with placeholder replacement and conditional rendering.

### Key Features

- **Template Management**: Store and manage templates
- **Placeholder Replacement**: Replace placeholders with actual data
- **Conditional Rendering**: Support for conditional content rendering
- **Loops**: Iterative content generation
- **Formatting**: Template formatting and styling
- **Multiple Formats**: Support for various output formats

## Installation

```bash
dotnet add package Mamey.Templates
```

## Usage

### Basic Setup

```csharp
using Mamey.Templates;
using Mamey;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddTemplates();
```

### Generate from Template

```csharp
public class DocumentService
{
    private readonly ITemplateService _templateService;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(ITemplateService templateService, ILogger<DocumentService> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<byte[]> GenerateFromTemplateAsync(string templateName, object data)
    {
        try
        {
            var document = await _templateService.GenerateAsync(templateName, data);
            _logger.LogInformation("Successfully generated document from template: {Template}", templateName);
            return document;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate document from template: {Template}", templateName);
            throw;
        }
    }
}
```

### Template Example

```csharp
// Template: invoice-template.html
/*
<h1>Invoice #{{InvoiceNumber}}</h1>
<p>Date: {{InvoiceDate}}</p>
<p>Customer: {{CustomerName}}</p>
<table>
    {{#Items}}
    <tr>
        <td>{{ItemName}}</td>
        <td>{{Quantity}}</td>
        <td>{{Price}}</td>
    </tr>
    {{/Items}}
</table>
<p>Total: {{Total}}</p>
*/

// Usage
var invoiceData = new
{
    InvoiceNumber = "INV-001",
    InvoiceDate = DateTime.Now,
    CustomerName = "John Doe",
    Items = new[]
    {
        new { ItemName = "Product 1", Quantity = 2, Price = 10.00m },
        new { ItemName = "Product 2", Quantity = 1, Price = 20.00m }
    },
    Total = 40.00m
};

var document = await _templateService.GenerateAsync("invoice-template", invoiceData);
```

## Template Features

- **Placeholder Replacement**: Replace `{{Placeholder}}` with actual values
- **Conditional Rendering**: Support for `{{#Condition}}...{{/Condition}}`
- **Loops**: Iterate over collections with `{{#Items}}...{{/Items}}`
- **Variables**: Use variables and expressions in templates
- **Formatting**: Support for custom formatting and styling
- **Multiple Formats**: Generate HTML, PDF, Word, Excel, and more

## Related Libraries

- **Mamey.Excel** - Excel template support
- **Mamey.Word** - Word template support
- **Mamey.Emails** - Email template support

## Additional Resources

- [Mamey Framework Documentation](../../documentation/)
- [Mamey.Templates Memory Documentation](../../.skmemory/v1/memory/public/mid-term/libraries/document-processing/mamey-templates.md)

