using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;
using System.Security.Claims;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Infrastructure.Mongo.Documents;

internal class SubjectDocument : IIdentifiable<Guid>
{
    public SubjectDocument()
    {

    }

    public SubjectDocument(Subject subject)
    {
        if (subject is null)
        {
            throw new NullReferenceException();
        }

        Id = subject.Id.Value;
        Name = subject.Name;
        Email = subject.Email;
        CreatedAt = subject.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = subject.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = subject.Tags;
        Version = subject.Version;
        Roles = subject.Roles;
        Status = subject.Status.ToString();
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public string Status { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public IEnumerable<RoleId> Roles { get; set; }
    public int Version { get; set; }

    public Subject AsEntity()
        => new(Id, Name, Email, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Roles, Status.ToEnum<SubjectStatus>(), Version);

    public SubjectDto AsDto()
        => new SubjectDto(Id, Name, Tags);
    public SubjectDetailsDto AsDetailsDto()
        => new SubjectDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

