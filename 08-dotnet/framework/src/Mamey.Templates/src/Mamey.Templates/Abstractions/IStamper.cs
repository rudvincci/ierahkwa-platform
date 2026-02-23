using Mamey.Templates.Configuration;

namespace Mamey.Templates.Abstractions;

internal interface IStamper
{
    Task<string> ApplyAsync(string docxPath, StampOptions? stamp, string? watermark, QrOptions? qr, CancellationToken ct);
}