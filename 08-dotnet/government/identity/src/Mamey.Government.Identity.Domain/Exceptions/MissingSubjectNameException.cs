using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingSubjectNameException : DomainException
{
    public MissingSubjectNameException()
        : base("Subject name is missing.")
    {
    }

    public override string Code => "missing_subject_name";
}
