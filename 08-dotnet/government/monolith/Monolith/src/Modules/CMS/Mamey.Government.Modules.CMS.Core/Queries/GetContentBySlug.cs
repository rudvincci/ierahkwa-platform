using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CMS.Core.DTO;

namespace Mamey.Government.Modules.CMS.Core.Queries;

internal class GetContentBySlug : IQuery<ContentDto?>
{
    public string Slug { get; set; } = string.Empty;
}
