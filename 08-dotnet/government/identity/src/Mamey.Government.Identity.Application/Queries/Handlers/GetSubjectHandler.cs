using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetSubjectHandler : IQueryHandler<Mamey.Government.Identity.Contracts.Queries.GetSubject, SubjectDetailsDto>
{
    private readonly ISubjectRepository _subjectRepository;

    public GetSubjectHandler(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<SubjectDetailsDto> HandleAsync(Mamey.Government.Identity.Contracts.Queries.GetSubject query, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(query.Id, cancellationToken);
        
        if (subject is null)
        {
            throw new SubjectNotFoundException(query.Id);
        }

        return MapToSubjectDetailsDto(subject);
    }

    private static SubjectDetailsDto MapToSubjectDetailsDto(Subject subject)
    {
        return new SubjectDetailsDto(
            subject.Id,
            subject.Name,
            subject.Email,
            subject.Status.ToString(),
            subject.Tags,
            subject.Roles.Select(r => r.Value),
            subject.LastAuthenticatedAt,
            subject.CreatedAt,
            subject.ModifiedAt
        );
    }
}
