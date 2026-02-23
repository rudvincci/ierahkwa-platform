namespace Mamey.Portal.Citizenship.Application.Models;

/// <summary>
/// Field types for AAMVA truncation rules
/// </summary>
public enum FieldType
{
    /// <summary>
    /// Name field (Family Name, Given Name, Middle Names)
    /// Truncate from right, preserve beginning
    /// </summary>
    Name,

    /// <summary>
    /// Address field (Street Address, City)
    /// Truncate from right, preserve street number
    /// </summary>
    Address,

    /// <summary>
    /// Postal code field
    /// Remove hyphens if needed, truncate from right
    /// </summary>
    PostalCode,

    /// <summary>
    /// License number field
    /// Never truncate - must be valid format
    /// </summary>
    LicenseNumber,

    /// <summary>
    /// Generic text field
    /// Truncate from right
    /// </summary>
    Text
}


