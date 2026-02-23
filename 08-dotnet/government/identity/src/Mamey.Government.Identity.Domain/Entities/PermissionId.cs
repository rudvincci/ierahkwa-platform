using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class PermissionId : AggregateId<Guid>
{
    public PermissionId(Guid value) : base(value) { }
    
    public static implicit operator PermissionId(Guid value) => new(value);
    public static implicit operator Guid(PermissionId id) => id.Value;
}