namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

/// <summary>
/// Citizenship application status progression.
/// </summary>
public enum ApplicationStatus
{
    Draft = 0,              // Application being filled out
    Submitted = 1,          // Application submitted for processing
    Validating = 2,         // Data validation in progress
    KycPending = 3,        // KYC verification pending
    ReviewPending = 4,     // Agent review pending
    Approved = 5,           // Application approved
    Rejected = 6           // Application rejected
}
