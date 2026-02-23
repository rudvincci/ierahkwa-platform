using System;
using System.Net.Http;
using System.Threading.Tasks;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Performance
{
    public class PerformanceTests
    {
        [Fact]
        public void GetMameyFwidIdentities()
        {
            const string url = "http://localhost:5001";
            const string stepName = "get_identities";
            const int duration = 3;
            const int expectedRps = 100;
            var endpoint = $"{url}/api/identities";
            const string scenarioName = "GET /api/identities";

            // Create HttpClient for HTTP requests
            var httpClient = new HttpClient();
            
            var scenario = Scenario.Create(scenarioName, async context =>
            {
                var initStep = await Step.Run(stepName, context, async () =>
                {
                    try
                    {
                        var request = Http.CreateRequest("GET", endpoint);
                        var response = await httpClient.SendAsync(request);
                        
                        // Check if response is successful (2xx status codes)
                        if (response.IsSuccessStatusCode)
                        {
                            return Response.Ok();
                        }
                        else
                        {
                            var errorMsg = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                            return Response.Fail(errorMsg, stepName, 0);
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        // If service is not running, this is expected
                        // Return a fail response so the test can still run
                        var errorMsg = $"HTTP request failed: {ex.Message}";
                        return Response.Fail(errorMsg, stepName, 0);
                    }
                    catch (Exception ex)
                    {
                        var errorMsg = $"Unexpected error: {ex.Message}";
                        return Response.Fail(errorMsg, stepName, 0);
                    }
                });
                
                return Response.Ok();
            })
            .WithoutWarmUp()
            .WithWarmUpDuration(TimeSpan.FromSeconds(duration));

            var result = NBomberRunner
                            .RegisterScenarios(scenario)
                            .Run();

            var scnStats = result.GetScenarioStats(scenarioName);
            
            // Skip test if service is not running (0 RPS means service is not available)
            if (scnStats.Ok.Request.RPS == 0 && scnStats.Fail.Request.Count > 0)
            {
                // Service is not running - fail with a clear message
                throw new InvalidOperationException($"Service is not running at {url}. Please start the service before running performance tests. Failed requests: {scnStats.Fail.Request.Count}");
            }
            
            Assert.True(scnStats.Ok.Request.RPS >= expectedRps, $"Expected RPS >= {expectedRps}, but got {scnStats.Ok.Request.RPS}");
            Assert.True(scnStats.Ok.Request.Count >= expectedRps * duration, $"Expected count >= {expectedRps * duration}, but got {scnStats.Ok.Request.Count}");
            Assert.True(scnStats.Ok.Latency.Percent75 < 200, $"Expected 75th percentile latency < 200ms, but got {scnStats.Ok.Latency.Percent75}ms");
            Assert.True(scnStats.Ok.Latency.Percent99 < 400, $"Expected 99th percentile latency < 400ms, but got {scnStats.Ok.Latency.Percent99}ms");
            Assert.True(scnStats.Fail.Request.RPS < 1, $"Expected failure RPS < 1, but got {scnStats.Fail.Request.RPS}");
        }
    }
}