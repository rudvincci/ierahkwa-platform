namespace Mamey.Portal.Cms.Domain.ValueObjects;

public readonly record struct CmsSlug(string Value)
{
    public override string ToString() => Value;
}
