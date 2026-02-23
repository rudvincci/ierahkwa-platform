using System.Runtime.CompilerServices;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Queries;

public class GetUserByEmail : IQuery<UserDto>
{
    public GetUserByEmail(string email)
    {
        Email = email;
    }

    public string Email { get; set; }
}
