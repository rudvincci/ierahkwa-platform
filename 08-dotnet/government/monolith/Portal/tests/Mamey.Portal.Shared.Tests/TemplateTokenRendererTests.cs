using Mamey.Portal.Shared.Storage.Templates;

namespace Mamey.Portal.Shared.Tests;

public sealed class TemplateTokenRendererTests
{
    [Fact]
    public void Apply_ReplacesKnownTokens_CaseInsensitive()
    {
        var tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["FullName"] = "Jane Doe",
            ["ApplicationNumber"] = "APP-20260111-ABC123",
        };

        var template = "Hello {{fullname}} (#{{APPLICATIONNUMBER}})";
        var result = TemplateTokenRenderer.Apply(template, tokens);

        Assert.Equal("Hello Jane Doe (#APP-20260111-ABC123)", result);
    }

    [Fact]
    public void Apply_PreservesUnknownTokens()
    {
        var tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["FullName"] = "Jane Doe",
        };

        var template = "Hello {{FullName}} {{UnknownToken}}";
        var result = TemplateTokenRenderer.Apply(template, tokens);

        Assert.Equal("Hello Jane Doe {{UnknownToken}}", result);
    }
}
