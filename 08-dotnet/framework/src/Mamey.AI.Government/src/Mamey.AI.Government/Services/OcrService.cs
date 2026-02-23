using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Services;

public class OcrService
{
    private readonly ILogger<OcrService> _logger;

    public OcrService(ILogger<OcrService> logger)
    {
        _logger = logger;
    }

    public async Task<Dictionary<string, string>> ExtractTextAsync(Stream documentStream, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Extracting text from document...");
        await Task.Delay(200, cancellationToken);

        // Stub extraction
        return new Dictionary<string, string>
        {
            { "FirstName", "John" },
            { "LastName", "Doe" },
            { "DocumentNumber", "A12345678" },
            { "DateOfBirth", "1980-01-01" }
        };
    }
}
