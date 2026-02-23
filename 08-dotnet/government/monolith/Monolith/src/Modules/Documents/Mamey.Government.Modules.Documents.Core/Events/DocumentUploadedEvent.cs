using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Documents.Core.Events;

public record DocumentUploadedEvent(Guid DocumentId, Guid TenantId, string FileName) : IEvent;
