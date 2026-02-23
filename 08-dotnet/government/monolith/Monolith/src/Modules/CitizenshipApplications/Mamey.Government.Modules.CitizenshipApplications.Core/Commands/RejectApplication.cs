using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands;

/// <summary>
/// Command to reject a citizenship application.
/// RejectedBy is automatically captured from the authenticated user context.
/// </summary>
public record RejectApplication(
    Guid ApplicationId,
    string Reason) : ICommand;
