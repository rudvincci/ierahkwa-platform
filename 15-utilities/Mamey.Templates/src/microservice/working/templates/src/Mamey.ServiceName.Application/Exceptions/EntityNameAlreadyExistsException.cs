using Mamey.Exceptions;
using Mamey.Types;
using Mamey.ServiceName.Domain.Entities;

namespace Mamey.ServiceName.Application.Exceptions;

internal class EntityNameAlreadyExistsException : MameyException
{
    public EntityNameAlreadyExistsException(EntityNameId entitynameId)
        : base($"EntityName with ID: '{entitynameId.Value}' already exists.")
        => EntityNameId = entitynameId;

    public EntityNameId EntityNameId { get; }
}
