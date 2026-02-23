namespace Mamey.Identity.AspNetCore.Managers;

/// <summary>
/// Reads the current impersonation context (if any).
/// </summary>
public interface IImpersonationContext
{
    bool  IsImpersonating { get; }
    Guid? OriginalUserId  { get; }
    Guid? ImpersonatedUserId { get; }
}

