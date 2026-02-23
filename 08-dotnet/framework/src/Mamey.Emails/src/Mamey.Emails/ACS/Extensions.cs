using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Emails.ACS;

public static class Extensions
{
    private const string SectionName = "email:acs";

    public static FluentEmailServicesBuilder AddACS(this FluentEmailServicesBuilder emailBuilder,
        string sectionName = SectionName)
    {
        emailBuilder
            .AddRazorRenderer()
            .AddACSEmailSender();
        return emailBuilder;
    }
}