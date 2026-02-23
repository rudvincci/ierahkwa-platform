using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.MicroMonolith.Infrastructure.Security.Encryption;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

internal sealed class ApplicationNumberService : IApplicationNumberService
{
    private readonly IApplicationRepository _repository;
    private readonly IRng _rng;
    private const string RandomChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int RandomLength = 6;

    public ApplicationNumberService(IApplicationRepository repository, IRng rng)
    {
        _repository = repository;
        _rng = rng;
    }

    public async Task<string> GenerateAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = GenerateRandomString(RandomLength);
        
        return $"{prefix}-{date}-{random}";
    }

    private string GenerateRandomString(int length)
    {
        // Generate random Base64 string (without removing special chars to keep it valid Base64)
        // We need at least 'length' bytes, Base64 encoding is ~4/3 size, so generate enough
        var randomBase64 = _rng.Generate((int)Math.Ceiling(length * 1.5), removeSpecialChars: false);
        
        // Decode Base64 back to bytes to get the original random bytes
        var bytes = Convert.FromBase64String(randomBase64);
        
        // Map bytes to our character set (A-Z, 0-9) using modulo
        return new string(bytes.Select(b => RandomChars[b % RandomChars.Length]).Take(length).ToArray());
    }
}
