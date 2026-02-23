using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Bookstore.Application.Commands;
using Pupitre.Bookstore.Application.Exceptions;
using Pupitre.Bookstore.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddBook),     
                new HandlerLogTemplate
                {
                    After = "Added a book with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(BookAlreadyExistsException), "Book with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteBook),  new HandlerLogTemplate { After = "Deleted a book with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}