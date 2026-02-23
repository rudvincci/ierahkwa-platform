using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Pupitre.AIAssessments.Tests.Shared.Factories
{
    public class MameyApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint: class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
            => base.CreateWebHostBuilder().UseEnvironment("tests");
    }
}