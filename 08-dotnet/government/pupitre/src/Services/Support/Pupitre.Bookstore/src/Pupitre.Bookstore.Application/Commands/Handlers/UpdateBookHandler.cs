using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Bookstore.Application.Exceptions;
using Pupitre.Bookstore.Contracts.Commands;
using Pupitre.Bookstore.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Bookstore.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateBookHandler : ICommandHandler<UpdateBook>
{
    private readonly IBookRepository _bookRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateBookHandler(
        IBookRepository bookRepository,
        IEventProcessor eventProcessor)
    {
        _bookRepository = bookRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateBook command, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetAsync(command.Id);

        if(book is null)
        {
            throw new BookNotFoundException(command.Id);
        }

        book.Update(command.Name, command.Tags);
        await _bookRepository.UpdateAsync(book);
        await _eventProcessor.ProcessAsync(book.Events);
    }
}


