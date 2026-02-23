using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands;

/// <summary>
/// Command to approve a citizenship application.
/// ApprovedBy is automatically captured from the authenticated user context.
/// </summary>
public record ApproveApplication(
    Guid ApplicationId,
    string? Notes = null) : ICommand;
