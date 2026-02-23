using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

internal interface ISubjectRepository
{
    Task AddAsync(Subject subject, CancellationToken cancellationToken = default);
    Task UpdateAsync(Subject subject, CancellationToken cancellationToken = default);
    Task DeleteAsync(SubjectId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Subject>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<Subject>> BrowseAsync(Func<Subject, bool> predicate, PagedQueryBase query, CancellationToken cancellationToken = default);
    Task<Subject> GetAsync(SubjectId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(SubjectId id, CancellationToken cancellationToken = default);
    
    // Extended Subject queries
    Task<Subject> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Subject>> GetByStatusAsync(SubjectStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Subject>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Subject>> GetByTagAsync(string tag, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Subject>> GetActiveSubjectsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Subject>> GetRecentlyAuthenticatedAsync(DateTime since, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, SubjectId excludeId, CancellationToken cancellationToken = default);
    Task<bool> HasRoleAsync(SubjectId subjectId, RoleId roleId, CancellationToken cancellationToken = default);
    Task<bool> HasTagAsync(SubjectId subjectId, string tag, CancellationToken cancellationToken = default);
    
    // Additional query methods
    Task<Subject> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
}
