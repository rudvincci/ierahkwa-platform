using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Mamey.FWID.Identities.Tests.Shared.Factories
{
    public class MameyApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint: class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
            => base.CreateWebHostBuilder().UseEnvironment("tests");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Find the API project's output directory to locate deps.json
            // This is needed because WebApplicationFactory looks for testhost.deps.json
            // which should be generated from the API project's deps.json
            var assemblyLocation = typeof(TEntryPoint).Assembly.Location;
            var apiProjectPath = Path.GetDirectoryName(assemblyLocation);
            
            if (!string.IsNullOrEmpty(apiProjectPath) && Directory.Exists(apiProjectPath))
            {
                builder.UseContentRoot(apiProjectPath);
            }

            base.ConfigureWebHost(builder);
        }
    }
}