using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Bookstore.Application.Exceptions;
using Pupitre.Bookstore.Contracts.Commands;
using Pupitre.Bookstore.Domain.Entities;
using Pupitre.Bookstore.Domain.Repositories;

namespace Pupitre.Bookstore.Application.Commands.Handlers;

internal sealed class AddBookHandler : ICommandHandler<AddBook>
{
    private readonly IBookRepository _bookRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddBookHandler(IBookRepository bookRepository,
        IEventProcessor eventProcessor)
    {
        _bookRepository = bookRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddBook command, CancellationToken cancellationToken = default)
    {
        
        var book = await _bookRepository.GetAsync(command.Id);
        
        if(book is not null)
        {
            throw new BookAlreadyExistsException(command.Id);
        }

        book = Book.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _bookRepository.AddAsync(book, cancellationToken);
        await _eventProcessor.ProcessAsync(book.Events);
    }
}

