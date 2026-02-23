using Mamey.Types;

namespace Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;

public class ContentId : TypeId
{
    public ContentId(Guid value) : base(value)
    {
    }

    public static implicit operator ContentId(Guid id) => new(id);
    public static implicit operator Guid(ContentId id) => id.Value;
}
