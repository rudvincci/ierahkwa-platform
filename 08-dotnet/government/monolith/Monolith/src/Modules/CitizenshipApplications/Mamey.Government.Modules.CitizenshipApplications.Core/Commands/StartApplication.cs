using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands;

public record StartApplication(
    string Email,
    string? FirstName = null,
    string? LastName = null) : ICommand;