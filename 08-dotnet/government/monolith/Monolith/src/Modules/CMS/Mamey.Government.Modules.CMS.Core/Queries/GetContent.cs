using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CMS.Core.DTO;

namespace Mamey.Government.Modules.CMS.Core.Queries;

internal class GetContent : IQuery<ContentDto?>
{
    public Guid ContentId { get; set; }
}
