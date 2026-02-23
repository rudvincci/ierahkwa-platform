using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CMS.Core.Commands;

public record CreateContent(
    Guid TenantId,
    string Title,
    string Slug,
    string ContentType,
    string? Body = null,
    string? Excerpt = null) : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
