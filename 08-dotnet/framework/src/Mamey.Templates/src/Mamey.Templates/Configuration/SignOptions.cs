namespace Mamey.Templates.Configuration;

public record SignOptions(bool Enabled, string? Pkcs12Path, string? Pkcs12Password, string? Reason, string? Location);