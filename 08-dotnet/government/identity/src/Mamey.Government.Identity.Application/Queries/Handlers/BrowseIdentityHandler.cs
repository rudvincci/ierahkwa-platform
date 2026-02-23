using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class BrowseIdentityHandler : IQueryHandler<BrowseIdentity, PagedResult<SubjectDto>>
{
    private readonly ISubjectRepository _subjectRepository;

    public BrowseIdentityHandler(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<PagedResult<SubjectDto>?> HandleAsync(Mamey.Government.Identity.Contracts.Queries.BrowseIdentity query, CancellationToken cancellationToken = default)
    {
        var subjects = await _subjectRepository.BrowseAsync(s => 
            string.IsNullOrEmpty(query.Name) || s.Name.Contains(query.Name), query);
        
        return subjects?.Map(MapToSubjectDto);
    }

    private static SubjectDto MapToSubjectDto(Subject subject)
    {
        return new SubjectDto(
            subject.Id,
            subject.Name,
            subject.Tags
        );
    }
}
