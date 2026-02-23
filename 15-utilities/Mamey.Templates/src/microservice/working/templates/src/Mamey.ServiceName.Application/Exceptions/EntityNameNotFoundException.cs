using Mamey.Exceptions;
using Mamey.Types;
using Mamey.ServiceName.Domain.Entities;

namespace Mamey.ServiceName.Application.Exceptions;

internal class EntityNameNotFoundException : MameyException
{
    public EntityNameNotFoundException(EntityNameId entitynameId)
        : base($"EntityName with ID: '{entitynameId.Value}' was not found.")
        => EntityNameId = entitynameId;

    public EntityNameId EntityNameId { get; }
}

