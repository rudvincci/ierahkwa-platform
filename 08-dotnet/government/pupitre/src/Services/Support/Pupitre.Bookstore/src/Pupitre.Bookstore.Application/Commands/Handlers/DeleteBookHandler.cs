using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Bookstore.Application.Exceptions;
using Pupitre.Bookstore.Contracts.Commands;
using Pupitre.Bookstore.Domain.Repositories;

namespace Pupitre.Bookstore.Application.Commands.Handlers;

internal sealed class DeleteBookHandler : ICommandHandler<DeleteBook>
{
    private readonly IBookRepository _bookRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteBookHandler(IBookRepository bookRepository, 
    IEventProcessor eventProcessor)
    {
        _bookRepository = bookRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteBook command, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetAsync(command.Id, cancellationToken);

        if (book is null)
        {
            throw new BookNotFoundException(command.Id);
        }

        await _bookRepository.DeleteAsync(book.Id);
        await _eventProcessor.ProcessAsync(book.Events);
    }
}


