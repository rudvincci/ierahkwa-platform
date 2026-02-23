using System.ComponentModel.DataAnnotations;
using Mamey.Types;
using Microsoft.AspNetCore.Identity;

namespace Mamey.Persistence.SQL;



public abstract class EFEntity<TIdentifiable> : AggregateRoot<TIdentifiable>, IIdentifiable<TIdentifiable>
{
    public EFEntity(TIdentifiable id)
        : base(id)
    {
    }
    public EFEntity(TIdentifiable id, int version = 0)
        : base(id, version)
    {
        Id = id;
        Version = version;
    }
    public EFEntity(AggregateRoot<TIdentifiable> aggregateRoot)
        : base(aggregateRoot.Id, aggregateRoot.Version)
    {
        
    }

    [Key]
    public override TIdentifiable Id { get => base.Id; protected set => base.Id = value; }

    [ConcurrencyCheck]
    public override int Version { get => base.Version; protected set => base.Version = value; }
}

public abstract class EFEntity : EFEntity<AggregateId>  
{
    public EFEntity(AggregateId id, int version)
        : base(id, version)
    {

    }
}
public abstract class EFIdentityEntity : EFIdentityEntity<Guid>
{
    protected EFIdentityEntity()
    {
    }

    protected EFIdentityEntity(string userName, Guid organizationId) : base(userName, organizationId)
    {
    }
    
}
public abstract class EFIdentityEntity<TIdentifiable> : IdentityUser<TIdentifiable>, IIdentifiable<TIdentifiable>, IEFIdentityEntity where TIdentifiable : IEquatable<TIdentifiable>
{
    protected EFIdentityEntity()
    {
    }

    protected EFIdentityEntity(string userName, Guid organizationId) : base(userName)
    {
        OrganizationId = organizationId;
    }
    public Guid OrganizationId { get; set; }
}
