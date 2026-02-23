using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class CredentialId : AggregateId<Guid>
{
    public CredentialId(Guid value) : base(value) { }
    
    public static implicit operator CredentialId(Guid value) => new(value);
    public static implicit operator Guid(CredentialId id) => id.Value;
}