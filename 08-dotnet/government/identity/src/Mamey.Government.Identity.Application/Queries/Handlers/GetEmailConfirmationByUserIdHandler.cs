using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetEmailConfirmationByUserIdHandler : IQueryHandler<GetEmailConfirmationByUserId, EmailConfirmationDto>
{
    private readonly IEmailConfirmationRepository _emailConfirmationRepository;

    public GetEmailConfirmationByUserIdHandler(IEmailConfirmationRepository emailConfirmationRepository)
    {
        _emailConfirmationRepository = emailConfirmationRepository;
    }

    public async Task<EmailConfirmationDto> HandleAsync(GetEmailConfirmationByUserId query, CancellationToken cancellationToken = default)
    {
        var emailConfirmation = await _emailConfirmationRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        
        if (emailConfirmation is null)
        {
            throw new EmailConfirmationNotFoundException(query.UserId);
        }

        return MapToEmailConfirmationDto(emailConfirmation);
    }

    private static EmailConfirmationDto MapToEmailConfirmationDto(EmailConfirmation emailConfirmation)
    {
        return new EmailConfirmationDto(
            emailConfirmation.Id,
            emailConfirmation.UserId,
            emailConfirmation.Email,
            emailConfirmation.ConfirmationCode,
            emailConfirmation.ExpiresAt,
            emailConfirmation.IpAddress,
            emailConfirmation.UserAgent,
            emailConfirmation.Status.ToString(),
            emailConfirmation.CreatedAt,
            emailConfirmation.ConfirmedAt
        );
    }
}
