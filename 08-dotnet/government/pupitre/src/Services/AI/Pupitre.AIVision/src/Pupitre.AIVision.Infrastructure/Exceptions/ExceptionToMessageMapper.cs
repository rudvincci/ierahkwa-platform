using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.AIVision.Application.Events.Rejected;
using Pupitre.AIVision.Application.Exceptions;
using System;
using Pupitre.AIVision.Domain.Exceptions;
using Pupitre.AIVision.Application.Commands;
using Mamey.Exceptions;
using Pupitre.AIVision.Contracts.Commands;

namespace Pupitre.AIVision.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            VisionAnalysisAlreadyExistsException ex => message switch
            {
                AddVisionAnalysis cmd => new AddVisionAnalysisRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            VisionAnalysisNotFoundException ex => message switch
            {
                UpdateVisionAnalysis cmd => new UpdateVisionAnalysisRejected(ex.VisionAnalysisId, ex.Message, ex.Code),
                DeleteVisionAnalysis cmd => new DeleteVisionAnalysisRejected(ex.VisionAnalysisId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

