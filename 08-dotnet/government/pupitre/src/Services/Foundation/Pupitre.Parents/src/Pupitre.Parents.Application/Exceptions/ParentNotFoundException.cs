using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Parents.Domain.Entities;

namespace Pupitre.Parents.Application.Exceptions;

internal class ParentNotFoundException : MameyException
{
    public ParentNotFoundException(ParentId parentId)
        : base($"Parent with ID: '{parentId.Value}' was not found.")
        => ParentId = parentId;

    public ParentId ParentId { get; }
}

