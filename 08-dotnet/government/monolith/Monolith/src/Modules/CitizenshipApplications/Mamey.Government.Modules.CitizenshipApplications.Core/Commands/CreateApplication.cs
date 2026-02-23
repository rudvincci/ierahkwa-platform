using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands;

public record CreateApplication(
    Guid ApplicationId,
    Guid TenantId,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string? Email,
    string? Phone,
    string? Street,
    string? City,
    string? State,
    string? PostalCode,
    string? Country) : ICommand;
