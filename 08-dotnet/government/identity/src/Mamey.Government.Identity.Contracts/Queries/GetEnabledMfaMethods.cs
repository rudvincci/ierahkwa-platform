using System.Runtime.CompilerServices;
using Mamey.CQRS.Queries;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Queries;

public class GetEnabledMfaMethods : IQuery<IEnumerable<string>>
{
    public UserId UserId { get; set; }
}

