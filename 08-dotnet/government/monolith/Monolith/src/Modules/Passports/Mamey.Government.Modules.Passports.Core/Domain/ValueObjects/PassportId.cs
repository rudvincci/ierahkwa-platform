using Mamey.Types;

namespace Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;

public class PassportId : TypeId
{
    public PassportId(Guid value) : base(value)
    {
    }

    public static implicit operator PassportId(Guid id) => new(id);
    public static implicit operator Guid(PassportId id) => id.Value;
}
