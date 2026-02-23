using System;
using System.Threading.Tasks;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Xunit;

namespace Mamey.ServiceName.Tests.Performance
{
    public class PerformanceTests
    {
        [Fact]
        public void get_servicename()
        {
            const string url = "http://localhost:serviceport";
            const string stepName = "init";
            const int duration = 3;
            const int expectedRps = 100;
            var endpoint = $"{url}/servicename";
           
            var scenario = Scenario.Create("GET servicename", async context =>
            {
                var initStep = await Step.Run(stepName, context, async () =>
                {
                    var httpResquestMessage = Http.CreateRequest("GET", endpoint)
                   // .WithCheck(response => Task.FromResult(response.IsSuccessStatusCode))
                    ;
                    //httpResquestMessage.
                    await Task.Delay(1000);
                    return Response.Ok();
                });
                Assert.True(!initStep.IsError);
                
                return Response.Ok();
            })
            .WithoutWarmUp()
            .WithWarmUpDuration(TimeSpan.FromSeconds(duration));

            var result = NBomberRunner
                            .RegisterScenarios(scenario)
                            .Run();

            ScenarioStats scnStats = result.GetScenarioStats("GET citizens");
            Assert.True(scnStats.Ok.Request.RPS > 100);
            Assert.True(scnStats.Ok.Request.Count >= expectedRps * duration);
            Assert.True(scnStats.Ok.Latency.Percent75 < 200); // 75% of requests below 200ms.
            Assert.True(scnStats.Ok.Latency.Percent99 < 400); // 99% of requests below 400ms.
            Assert.True(scnStats.Fail.Request.RPS < 1);   // less than 1% of errors

            //result.

            //var scnStats = result.GetScenarioStats("GET servicename");

            //var step = HttpStep.Create(stepName, ctx =>
            //    Task.FromResult(Http.CreateRequest("GET", endpoint)
            //        .WithCheck(response => Task.FromResult(response.IsSuccessStatusCode))));

            //var assertions = new[]
            //{
            //    Assertion.ForStep(stepName, s => s.RPS >= expectedRps),
            //    Assertion.ForStep(stepName, s => s.OkCount >= expectedRps * duration)
            //};

            //var scenario = ScenarioBuilder.CreateScenario("GET servicename", new[] {step})
            //    .WithConcurrentCopies(1)
            //    .WithOutWarmUp()
            //    .WithDuration(TimeSpan.FromSeconds(duration))
            //    .WithAssertions(assertions);

            //NBomberRunner.RegisterScenarios(scenario)
            //    .RunTest();
        }
    }
}