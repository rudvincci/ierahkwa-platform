using System;

namespace Mamey.Government.Modules.Passports.Core.Clients.DTO;

/// <summary>
/// DTO for citizen data received from the Citizens module.
/// </summary>
internal class CitizenDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? Status { get; set; }
}
