using System.Runtime.CompilerServices;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Queries;

public class GetUserSessions : IQuery<IEnumerable<SessionDto>>
{
    public UserId UserId { get; set; }
}

