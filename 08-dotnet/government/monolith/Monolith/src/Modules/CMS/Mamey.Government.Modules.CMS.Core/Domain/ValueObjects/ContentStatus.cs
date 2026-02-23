namespace Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;

/// <summary>
/// Content status for CMS publishing workflow.
/// </summary>
public enum ContentStatus
{
    Draft = 0,
    Review = 1,
    Published = 2,
    Archived = 3
}
