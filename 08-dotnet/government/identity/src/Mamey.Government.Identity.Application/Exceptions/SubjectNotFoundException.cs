using Mamey.Exceptions;
using Mamey.Types;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Application.Exceptions;

internal class SubjectNotFoundException : MameyException
{
    public SubjectNotFoundException(SubjectId subjectId)
        : base($"Subject with ID: '{subjectId.Value}' was not found.")
        => SubjectId = subjectId;
    public SubjectNotFoundException(string email)
        : base($"Subject with Email: '{email}' was not found.")
        => Email = email;
    public SubjectId? SubjectId { get; }
    public string? Email { get; }
}

