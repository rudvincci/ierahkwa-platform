using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Documents.Core.DTO;

namespace Mamey.Government.Modules.Documents.Core.Queries;

internal class GetDocument : IQuery<DocumentDto?>
{
    public Guid DocumentId { get; set; }
}
