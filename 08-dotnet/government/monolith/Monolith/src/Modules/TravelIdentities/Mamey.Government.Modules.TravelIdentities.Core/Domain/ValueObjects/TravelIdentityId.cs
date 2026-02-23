using Mamey.Types;

namespace Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;

public class TravelIdentityId : TypeId
{
    public TravelIdentityId(Guid value) : base(value)
    {
    }

    public static implicit operator TravelIdentityId(Guid id) => new(id);
    public static implicit operator Guid(TravelIdentityId id) => id.Value;
}
