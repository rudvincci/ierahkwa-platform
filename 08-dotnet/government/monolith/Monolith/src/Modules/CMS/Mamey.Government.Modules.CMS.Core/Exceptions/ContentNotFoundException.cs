using System;
using Mamey.Exceptions;

namespace Mamey.Government.Modules.CMS.Core.Exceptions;

public sealed class ContentNotFoundException : MameyException
{
    public Guid ContentId { get; }

    public ContentNotFoundException(Guid contentId)
        : base($"Content with ID '{contentId}' was not found.")
    {
        ContentId = contentId;
    }
}
