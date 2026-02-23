namespace Mamey.Templates;

public sealed record TemplateId(string Value)
{
    public override string ToString() => Value;
    public static TemplateId Parse(string s) => new(s.Trim());
}