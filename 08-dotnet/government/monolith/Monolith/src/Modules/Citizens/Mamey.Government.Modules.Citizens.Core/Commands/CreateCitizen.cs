using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Citizens.Core.Commands;

public record CreateCitizen(
    Guid CitizenId,
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
    string? Country,
    string? PhotoUrl,
    Guid? ApplicationId = null) : ICommand;
