using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Users.Application.Events.Rejected;
using Pupitre.Users.Application.Exceptions;
using Pupitre.Users.Domain.Exceptions;
using Pupitre.Users.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Users.Contracts.Commands;

namespace Pupitre.Users.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            UserAlreadyExistsException ex => message switch
            {
                AddUser cmd => new AddUserRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            UserNotFoundException ex => message switch
            {
                UpdateUser cmd => new UpdateUserRejected(ex.UserId, ex.Message, ex.Code),
                DeleteUser cmd => new DeleteUserRejected(ex.UserId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}
