using Mamey.Types;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

public class ApplicationId : AggregateId<Guid>
{
    public ApplicationId(Guid value) : base(value)
    {
    }

    public static implicit operator ApplicationId(Guid id) => new(id);
    public static implicit operator Guid(ApplicationId id) => id.Value;
}
