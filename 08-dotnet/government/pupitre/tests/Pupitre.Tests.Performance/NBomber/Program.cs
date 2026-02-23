using NBomber.CSharp;
using NBomber.Contracts;

var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:60000";
var authToken = Environment.GetEnvironmentVariable("AUTH_TOKEN") ?? "";

using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(baseUrl);
if (!string.IsNullOrEmpty(authToken))
{
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
}

// Users API Scenario
var getUsersScenario = Scenario.Create("get_users", async context =>
{
    try
    {
        var response = await httpClient.GetAsync("/api/users?page=1&pageSize=10");
        return response.IsSuccessStatusCode 
            ? Response.Ok() 
            : Response.Fail();
    }
    catch
    {
        return Response.Fail();
    }
})
.WithWarmUpDuration(TimeSpan.FromSeconds(10))
.WithLoadSimulations(
    Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)),
    Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
    Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(2)),
    Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
    Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
);

// GLEs API Scenario
var getGLEsScenario = Scenario.Create("get_gles", async context =>
{
    try
    {
        var response = await httpClient.GetAsync("/api/gles?page=1&pageSize=20");
        return response.IsSuccessStatusCode 
            ? Response.Ok() 
            : Response.Fail();
    }
    catch
    {
        return Response.Fail();
    }
})
.WithWarmUpDuration(TimeSpan.FromSeconds(10))
.WithLoadSimulations(
    Simulation.Inject(rate: 20, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)),
    Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
    Simulation.Inject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(2)),
    Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
    Simulation.Inject(rate: 20, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
);

// Lessons API Scenario
var getLessonsScenario = Scenario.Create("get_lessons", async context =>
{
    try
    {
        var response = await httpClient.GetAsync("/api/lessons?page=1&pageSize=10");
        return response.IsSuccessStatusCode 
            ? Response.Ok() 
            : Response.Fail();
    }
    catch
    {
        return Response.Fail();
    }
})
.WithWarmUpDuration(TimeSpan.FromSeconds(10))
.WithLoadSimulations(
    Simulation.Inject(rate: 30, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)),
    Simulation.Inject(rate: 150, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
    Simulation.Inject(rate: 300, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(2)),
    Simulation.Inject(rate: 150, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
    Simulation.Inject(rate: 30, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
);

// AI Tutor Scenario (lower load)
var aiTutorScenario = Scenario.Create("ai_tutor_chat", async context =>
{
    var questions = new[]
    {
        "What is 2 + 2?",
        "Explain photosynthesis.",
        "What is the capital of France?",
    };

    var chatPayload = $$"""
    {
        "sessionId": "perf-test-session",
        "message": "{{questions[context.ScenarioInfo.InstanceNumber % questions.Length]}}"
    }
    """;

    try
    {
        var content = new StringContent(chatPayload, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("/api/ai/tutors/chat", content);
        return response.IsSuccessStatusCode 
            ? Response.Ok() 
            : Response.Fail();
    }
    catch
    {
        return Response.Fail();
    }
})
.WithWarmUpDuration(TimeSpan.FromSeconds(10))
.WithLoadSimulations(
    Simulation.Inject(rate: 5, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)),
    Simulation.Inject(rate: 20, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
    Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(2)),
    Simulation.Inject(rate: 20, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
    Simulation.Inject(rate: 5, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
);

NBomberRunner
    .RegisterScenarios(getUsersScenario, getGLEsScenario, getLessonsScenario, aiTutorScenario)
    .WithReportFileName("pupitre-load-test-report")
    .WithReportFolder("./reports")
    .WithReportFormats(ReportFormat.Txt, ReportFormat.Html, ReportFormat.Csv)
    .Run();
