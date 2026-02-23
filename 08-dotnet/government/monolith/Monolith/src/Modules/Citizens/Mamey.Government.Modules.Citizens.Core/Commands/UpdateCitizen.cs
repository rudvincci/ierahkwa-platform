using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Citizens.Core.Commands;

public record UpdateCitizen(
    Guid CitizenId,
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? Street,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    string? PhotoUrl) : ICommand;
