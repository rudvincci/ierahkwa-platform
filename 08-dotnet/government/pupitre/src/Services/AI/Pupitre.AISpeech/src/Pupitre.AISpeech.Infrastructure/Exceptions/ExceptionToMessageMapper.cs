using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.AISpeech.Application.Events.Rejected;
using Pupitre.AISpeech.Application.Exceptions;
using System;
using Pupitre.AISpeech.Domain.Exceptions;
using Pupitre.AISpeech.Application.Commands;
using Mamey.Exceptions;
using Pupitre.AISpeech.Contracts.Commands;

namespace Pupitre.AISpeech.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            SpeechRequestAlreadyExistsException ex => message switch
            {
                AddSpeechRequest cmd => new AddSpeechRequestRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            SpeechRequestNotFoundException ex => message switch
            {
                UpdateSpeechRequest cmd => new UpdateSpeechRequestRejected(ex.SpeechRequestId, ex.Message, ex.Code),
                DeleteSpeechRequest cmd => new DeleteSpeechRequestRejected(ex.SpeechRequestId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

