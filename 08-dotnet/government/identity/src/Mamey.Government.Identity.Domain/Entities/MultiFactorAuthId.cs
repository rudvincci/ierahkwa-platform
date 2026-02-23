using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class MultiFactorAuthId : AggregateId<Guid>
{
    public MultiFactorAuthId(Guid value) : base(value) { }
    
    public static implicit operator MultiFactorAuthId(Guid value) => new(value);
    public static implicit operator Guid(MultiFactorAuthId id) => id.Value;
}