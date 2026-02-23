using Pupitre.Bookstore.Application.Commands;
using Pupitre.Bookstore.Application.Events;
using Pupitre.Bookstore.Contracts.Commands;
using Pupitre.Bookstore.Infrastructure.Mongo.Documents;
using Pupitre.Bookstore.Tests.Shared.Factories;
using Pupitre.Bookstore.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Bookstore.Tests.Integration.Async;

public class AddBookTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddBook command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_book_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddBook(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<BookAdded, BookDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "bookstore";
    private readonly MongoDbFixture<BookDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddBookTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<BookDocument, Guid>("bookstore");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}