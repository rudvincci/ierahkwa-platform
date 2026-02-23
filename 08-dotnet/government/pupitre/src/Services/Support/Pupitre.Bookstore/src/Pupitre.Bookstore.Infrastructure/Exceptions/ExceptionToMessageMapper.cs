using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Bookstore.Application.Events.Rejected;
using Pupitre.Bookstore.Application.Exceptions;
using System;
using Pupitre.Bookstore.Domain.Exceptions;
using Pupitre.Bookstore.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Bookstore.Contracts.Commands;

namespace Pupitre.Bookstore.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            BookAlreadyExistsException ex => message switch
            {
                AddBook cmd => new AddBookRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            BookNotFoundException ex => message switch
            {
                UpdateBook cmd => new UpdateBookRejected(ex.BookId, ex.Message, ex.Code),
                DeleteBook cmd => new DeleteBookRejected(ex.BookId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

