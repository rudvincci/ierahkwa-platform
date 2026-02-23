using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands;

public record ResumeApplication(
    string Token,
    string Email) : ICommand;