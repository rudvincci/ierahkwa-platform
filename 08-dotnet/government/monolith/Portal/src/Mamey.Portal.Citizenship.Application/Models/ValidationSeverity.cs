namespace Mamey.Portal.Citizenship.Application.Models;

public enum ValidationSeverity
{
    /// <summary>
    /// Information only - no action required
    /// </summary>
    Info,

    /// <summary>
    /// Warning - field was corrected but may need review
    /// </summary>
    Warning,

    /// <summary>
    /// Error - validation failed and must be corrected
    /// </summary>
    Error
}


