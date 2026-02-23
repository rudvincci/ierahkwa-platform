using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Documents.Generators;

/// <summary>
/// Service for loading HTML templates for document generation.
/// </summary>
internal class TemplateService : ITemplateService
{
    private readonly ILogger<TemplateService> _logger;
    private readonly Dictionary<string, string> _templateCache = new();

    public TemplateService(ILogger<TemplateService> logger)
    {
        _logger = logger;
    }

    public async Task<string> LoadTemplateAsync(string templateName, CancellationToken cancellationToken = default)
    {
        // Check cache first
        if (_templateCache.TryGetValue(templateName, out var cachedTemplate))
        {
            return await Task.FromResult(cachedTemplate);
        }

        try
        {
            // Load template from embedded resources or file system
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Mamey.FWID.Identities.Application.Documents.Templates.{templateName}Template.html";

            string template;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    // Fallback: try loading from file system
                    var templatePath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Templates",
                        $"{templateName}Template.html");

                    if (File.Exists(templatePath))
                    {
                        template = await File.ReadAllTextAsync(templatePath, cancellationToken);
                    }
                    else
                    {
                        // Generate default template
                        template = GenerateDefaultTemplate(templateName);
                        _logger.LogWarning(
                            "Template not found: {TemplateName}, using default template",
                            templateName);
                    }
                }
                else
                {
                    using var reader = new StreamReader(stream);
                    template = await reader.ReadToEndAsync(cancellationToken);
                }
            }

            // Cache template
            _templateCache[templateName] = template;

            _logger.LogDebug(
                "Loaded template: {TemplateName}, Length: {Length}",
                templateName,
                template.Length);

            return template;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load template: {TemplateName}", templateName);
            // Return default template as fallback
            return GenerateDefaultTemplate(templateName);
        }
    }

    private string GenerateDefaultTemplate(string templateName)
    {
        return templateName switch
        {
            "IdCard" => GetDefaultIdCardTemplate(),
            "Passport" => GetDefaultPassportTemplate(),
            _ => "<html><body><h1>Document Template</h1></body></html>"
        };
    }

    private string GetDefaultIdCardTemplate()
    {
        return @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>ID Card</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .id-card { border: 2px solid #000; padding: 20px; max-width: 400px; }
        .header { text-align: center; font-weight: bold; margin-bottom: 20px; }
        .photo { width: 100px; height: 120px; border: 1px solid #000; float: right; }
        .info { margin: 10px 0; }
        .qr-code { text-align: center; margin-top: 20px; }
    </style>
</head>
<body>
    <div class=""id-card"">
        <div class=""header"">FUTUREWAMPUM ID CARD</div>
        <img src=""{{Photo}}"" class=""photo"" alt=""Photo"">
        <div class=""info""><strong>Name:</strong> {{FullName}}</div>
        <div class=""info""><strong>DOB:</strong> {{DateOfBirth}}</div>
        <div class=""info""><strong>Address:</strong> {{Address}}</div>
        <div class=""info""><strong>ID Number:</strong> {{DocumentNumber}}</div>
        <div class=""info""><strong>Issued:</strong> {{IssuedDate}}</div>
        <div class=""info""><strong>Expires:</strong> {{ExpiryDate}}</div>
        <div class=""qr-code"">
            <img src=""{{QrCode}}"" alt=""QR Code"" style=""width: 100px; height: 100px;"">
        </div>
    </div>
</body>
</html>";
    }

    private string GetDefaultPassportTemplate()
    {
        return @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Passport</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .passport { border: 2px solid #000; padding: 20px; max-width: 500px; }
        .header { text-align: center; font-weight: bold; margin-bottom: 20px; }
        .photo { width: 120px; height: 150px; border: 1px solid #000; float: right; }
        .info { margin: 10px 0; }
        .qr-code { text-align: center; margin-top: 20px; }
    </style>
</head>
<body>
    <div class=""passport"">
        <div class=""header"">FUTUREWAMPUM PASSPORT</div>
        <img src=""{{Photo}}"" class=""photo"" alt=""Photo"">
        <div class=""info""><strong>Passport No:</strong> {{PassportNumber}}</div>
        <div class=""info""><strong>Surname:</strong> {{LastName}}</div>
        <div class=""info""><strong>Given Names:</strong> {{FirstName}}</div>
        <div class=""info""><strong>Nationality:</strong> {{Nationality}}</div>
        <div class=""info""><strong>Date of Birth:</strong> {{DateOfBirth}}</div>
        <div class=""info""><strong>Place of Birth:</strong> {{PlaceOfBirth}}</div>
        <div class=""info""><strong>Date of Issue:</strong> {{IssuedDate}}</div>
        <div class=""info""><strong>Date of Expiry:</strong> {{ExpiryDate}}</div>
        <div class=""qr-code"">
            <img src=""{{QrCode}}"" alt=""QR Code"" style=""width: 100px; height: 100px;"">
        </div>
    </div>
</body>
</html>";
    }
}
