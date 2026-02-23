using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class TwoFactorAuthId : AggregateId<Guid>
{
    public TwoFactorAuthId(Guid value) : base(value) { }
    
    public static implicit operator TwoFactorAuthId(Guid value) => new(value);
    public static implicit operator Guid(TwoFactorAuthId id) => id.Value;
}