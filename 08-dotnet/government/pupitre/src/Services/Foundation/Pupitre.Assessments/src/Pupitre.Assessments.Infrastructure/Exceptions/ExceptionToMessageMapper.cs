using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Assessments.Application.Events.Rejected;
using Pupitre.Assessments.Application.Exceptions;
using System;
using Pupitre.Assessments.Domain.Exceptions;
using Pupitre.Assessments.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Assessments.Contracts.Commands;

namespace Pupitre.Assessments.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            AssessmentAlreadyExistsException ex => message switch
            {
                AddAssessment cmd => new AddAssessmentRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            AssessmentNotFoundException ex => message switch
            {
                UpdateAssessment cmd => new UpdateAssessmentRejected(ex.AssessmentId, ex.Message, ex.Code),
                DeleteAssessment cmd => new DeleteAssessmentRejected(ex.AssessmentId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

