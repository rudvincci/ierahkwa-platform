using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.AIAssessments.Application.Events.Rejected;
using Pupitre.AIAssessments.Application.Exceptions;
using System;
using Pupitre.AIAssessments.Domain.Exceptions;
using Pupitre.AIAssessments.Application.Commands;
using Mamey.Exceptions;
using Pupitre.AIAssessments.Contracts.Commands;

namespace Pupitre.AIAssessments.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            AIAssessmentAlreadyExistsException ex => message switch
            {
                AddAIAssessment cmd => new AddAIAssessmentRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            AIAssessmentNotFoundException ex => message switch
            {
                UpdateAIAssessment cmd => new UpdateAIAssessmentRejected(ex.AIAssessmentId, ex.Message, ex.Code),
                DeleteAIAssessment cmd => new DeleteAIAssessmentRejected(ex.AIAssessmentId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

