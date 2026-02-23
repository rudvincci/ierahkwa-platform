using Pupitre.Lessons.Application.Commands;
using Pupitre.Lessons.Application.Events;
using Pupitre.Lessons.Contracts.Commands;
using Pupitre.Lessons.Infrastructure.Mongo.Documents;
using Pupitre.Lessons.Tests.Shared.Factories;
using Pupitre.Lessons.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Lessons.Tests.Integration.Async;

public class AddLessonTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddLesson command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_lesson_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddLesson(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<LessonAdded, LessonDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "lessons";
    private readonly MongoDbFixture<LessonDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddLessonTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<LessonDocument, Guid>("lessons");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}