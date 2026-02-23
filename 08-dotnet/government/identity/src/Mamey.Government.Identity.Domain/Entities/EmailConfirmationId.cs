using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class EmailConfirmationId : AggregateId<Guid>
{
    public EmailConfirmationId(Guid value) : base(value) { }
    
    public static implicit operator EmailConfirmationId(Guid value) => new(value);
    public static implicit operator Guid(EmailConfirmationId id) => id.Value;
}