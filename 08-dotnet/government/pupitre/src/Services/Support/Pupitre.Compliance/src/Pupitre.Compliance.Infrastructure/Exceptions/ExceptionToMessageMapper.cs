using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Compliance.Application.Events.Rejected;
using Pupitre.Compliance.Application.Exceptions;
using System;
using Pupitre.Compliance.Domain.Exceptions;
using Pupitre.Compliance.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Compliance.Contracts.Commands;

namespace Pupitre.Compliance.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            ComplianceRecordAlreadyExistsException ex => message switch
            {
                AddComplianceRecord cmd => new AddComplianceRecordRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            ComplianceRecordNotFoundException ex => message switch
            {
                UpdateComplianceRecord cmd => new UpdateComplianceRecordRejected(ex.ComplianceRecordId, ex.Message, ex.Code),
                DeleteComplianceRecord cmd => new DeleteComplianceRecordRejected(ex.ComplianceRecordId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

