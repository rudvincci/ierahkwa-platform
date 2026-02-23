using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Citizens.Core.Commands;

public record ProgressCitizenStatus(
    Guid CitizenId,
    string NewStatus,
    string ApprovedBy,
    string? Reason = null) : ICommand;
