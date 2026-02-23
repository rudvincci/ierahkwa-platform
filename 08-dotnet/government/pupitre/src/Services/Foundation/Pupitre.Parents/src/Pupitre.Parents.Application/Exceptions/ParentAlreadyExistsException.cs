using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Parents.Domain.Entities;

namespace Pupitre.Parents.Application.Exceptions;

internal class ParentAlreadyExistsException : MameyException
{
    public ParentAlreadyExistsException(ParentId parentId)
        : base($"Parent with ID: '{parentId.Value}' already exists.")
        => ParentId = parentId;

    public ParentId ParentId { get; }
}
