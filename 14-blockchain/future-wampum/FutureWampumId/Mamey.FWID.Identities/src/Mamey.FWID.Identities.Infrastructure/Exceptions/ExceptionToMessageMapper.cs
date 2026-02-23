using System;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.FWID.Identities.Application.Events.Rejected;
using Mamey.FWID.Identities.Application.Exceptions;
using System;
using Mamey.FWID.Identities.Domain.Exceptions;
using Mamey.FWID.Identities.Application.Commands;
using Mamey.Exceptions;
using Mamey.FWID.Identities.Contracts.Commands;

namespace Mamey.FWID.Identities.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object? Map(Exception exception, object message)
        => exception switch
        {
            IdentityAlreadyExistsException ex => message switch
            {
                AddIdentity cmd => new AddIdentityRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null
            },
            IdentityNotFoundException ex => message switch
            {
                UpdateZone cmd => new UpdateZoneRejected(ex.IdentityId, ex.Message, ex.Code ?? "IDENTITY_NOT_FOUND"),
                UpdateContactInformation cmd => new UpdateContactInformationRejected(ex.IdentityId, ex.Message, ex.Code ?? "IDENTITY_NOT_FOUND"),
                _ => null!
            },
            _ => null!
        };
}

