using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mamey.Security.EntityFramework.ValueConverters;

/// <summary>
/// EF Core value converter for automatically hashing string properties marked with [HashedAttribute].
/// Note: Hashing is one-way, so reading from database returns the stored hash value.
/// </summary>
public class HashedValueConverter : ValueConverter<string, string>
{
    public HashedValueConverter(ISecurityProvider securityProvider)
        : base(
            v => (v == null || v.Length == 0) ? string.Empty : securityProvider.Hash(v),
            v => (v == null || v.Length == 0) ? string.Empty : (v ?? string.Empty)) // Hashing is one-way, return stored value as-is, but handle empty strings and null
    {
        if (securityProvider == null)
            throw new ArgumentNullException(nameof(securityProvider));
    }
}



