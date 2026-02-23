using Mamey.Exceptions;
using Mamey.Types;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Application.Exceptions;

internal class SubjectAlreadyExistsException : MameyException
{
    public SubjectAlreadyExistsException(SubjectId subjectId)
        : base($"Subject with ID: '{subjectId.Value}' already exists.")
        => SubjectId = subjectId;

    public SubjectId SubjectId { get; }
}
