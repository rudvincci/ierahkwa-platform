using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class MfaChallengeId : AggregateId<Guid>
{
    public MfaChallengeId(Guid value) : base(value) { }
    
    public static implicit operator MfaChallengeId(Guid value) => new(value);
    public static implicit operator Guid(MfaChallengeId id) => id.Value;
}