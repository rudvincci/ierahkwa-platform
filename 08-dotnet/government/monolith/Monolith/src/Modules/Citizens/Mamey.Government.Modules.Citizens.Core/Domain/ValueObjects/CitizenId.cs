using Mamey.Types;

namespace Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;

public class CitizenId : TypeId
{
    public CitizenId(Guid value) : base(value)
    {
    }

    public static implicit operator CitizenId(Guid id) => new(id);
    public static implicit operator Guid(CitizenId id) => id.Value;
}
