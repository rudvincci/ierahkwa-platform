using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mamey.Security.EntityFramework.ValueConverters;

/// <summary>
/// EF Core value converter for automatically encrypting/decrypting string properties marked with [EncryptedAttribute].
/// </summary>
public class EncryptedValueConverter : ValueConverter<string, string>
{
    public EncryptedValueConverter(ISecurityProvider securityProvider)
        : base(
            v => v == null ? string.Empty : securityProvider.Encrypt(v),
            v => (v == null || v.Length == 0) ? string.Empty : (securityProvider.Decrypt(v) ?? string.Empty))
    {
        if (securityProvider == null)
            throw new ArgumentNullException(nameof(securityProvider));
    }
}



