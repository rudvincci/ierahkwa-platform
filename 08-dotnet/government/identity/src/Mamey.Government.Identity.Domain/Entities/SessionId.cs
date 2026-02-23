using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class SessionId : AggregateId<Guid>
{
    public SessionId(Guid value) : base(value) { }
    
    public static implicit operator SessionId(Guid value) => new(value);
    public static implicit operator Guid(SessionId id) => id.Value;
}