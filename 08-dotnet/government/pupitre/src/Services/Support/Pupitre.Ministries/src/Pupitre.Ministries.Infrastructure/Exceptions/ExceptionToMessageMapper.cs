using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Ministries.Application.Events.Rejected;
using Pupitre.Ministries.Application.Exceptions;
using System;
using Pupitre.Ministries.Domain.Exceptions;
using Pupitre.Ministries.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Ministries.Contracts.Commands;

namespace Pupitre.Ministries.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            MinistryDataAlreadyExistsException ex => message switch
            {
                AddMinistryData cmd => new AddMinistryDataRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            MinistryDataNotFoundException ex => message switch
            {
                UpdateMinistryData cmd => new UpdateMinistryDataRejected(ex.MinistryDataId, ex.Message, ex.Code),
                DeleteMinistryData cmd => new DeleteMinistryDataRejected(ex.MinistryDataId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

