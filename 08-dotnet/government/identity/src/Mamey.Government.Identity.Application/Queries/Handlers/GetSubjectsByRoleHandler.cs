using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetSubjectsByRoleHandler : IQueryHandler<GetSubjectsByRole, IEnumerable<SubjectDetailsDto>>
{
    private readonly ISubjectRepository _subjectRepository;

    public GetSubjectsByRoleHandler(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<IEnumerable<SubjectDetailsDto>> HandleAsync(GetSubjectsByRole query, CancellationToken cancellationToken = default)
    {
        var subjects = await _subjectRepository.GetByRoleIdAsync(query.RoleId, cancellationToken);
        return subjects.Select(MapToSubjectDetailsDto);
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
