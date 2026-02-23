using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Passports.Core.Commands;

public record ReportLostStolen(
    Guid PassportId, 
    string ReportType, // "Lost" or "Stolen"
    string Description,
    string ReportedBy) : ICommand;
