using Mamey.Types;

namespace Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;

public class DocumentId : TypeId
{
    public DocumentId(Guid value) : base(value)
    {
    }

    public static implicit operator DocumentId(Guid id) => new(id);
    public static implicit operator Guid(DocumentId id) => id.Value;
}
