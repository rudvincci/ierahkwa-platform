using Mamey.Types;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

internal record EmploymentHistory(
    string Employer,
    string Occupation,
    DateTime EmployedFrom,
    DateTime? EmployedTo = null,
    Address? EmployerAddress = null,
    Phone? EmployerPhone = null);
