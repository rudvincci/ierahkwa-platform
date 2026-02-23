using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Infrastructure.Mongo.Documents;

internal class UserDocument : IIdentifiable<Guid>
{
    public UserDocument()
    {

    }

    public UserDocument(User user)
    {
        if (user is null)
        {
            throw new NullReferenceException();
        }

        Id = user.Id.Value;
        SubjectId = user.SubjectId.Value;
        Username = user.Username;
        Email = user.Email.Value;
        Status = user.Status.ToString();
        LastLoginAt = user.LastLoginAt?.ToUnixTimeMilliseconds();
        CreatedAt = user.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = user.ModifiedAt?.ToUnixTimeMilliseconds();
        Version = user.Version;
    }

    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
    
    public bool MultiFactorEnabled { get; set; }
    public long? MultiFactorEnabledAt { get; set; }
    public long? LastLoginAt { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public int Version { get; set; }

    public User AsEntity()
        => new(Id, SubjectId, Username, Email, "", 
               CreatedAt.GetDate(), ModifiedAt?.GetDate(), 
               Status.ToEnum<UserStatus>(), MultiFactorEnabled, MultiFactorEnabledAt?.GetDate(), Version);

    public UserDto AsDto()
        => new UserDto(Id, SubjectId, Username, Email, Status == "Active", Status == "Locked", 
                       null, LastLoginAt?.GetDate(), false, false, false, 
                       null, null, null, CreatedAt.GetDate(), ModifiedAt?.GetDate());
    
}
