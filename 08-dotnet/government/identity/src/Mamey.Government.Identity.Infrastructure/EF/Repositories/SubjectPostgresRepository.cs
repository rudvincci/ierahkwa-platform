using System.Collections.Immutable;
using Mamey.CQRS.Queries;
using Mamey.Persistence.SQL.Repositories;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Identity.Infrastructure.EF.Repositories;

internal class SubjectPostgresRepository : EFRepository<Subject, SubjectId>, ISubjectRepository, IDisposable
{
    private readonly UserDbContext _dbContext;

    public SubjectPostgresRepository(UserDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Subject>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var subjects = await _dbContext.Subjects.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(subjects);
    }

    public async Task<PagedResult<Subject>> BrowseAsync(Func<Subject, bool> predicate, PagedQueryBase query, CancellationToken cancellationToken = default)
    {
        var subjects = _dbContext.Subjects.AsEnumerable().Where(predicate);
        var totalCount = subjects.Count();
        var pagedSubjects = subjects
            .Skip((query.Page - 1) * query.ResultsPerPage)
            .Take(query.ResultsPerPage)
            .ToList();

        return PagedResult<Subject>.Create(pagedSubjects, query.Page, query.ResultsPerPage, 
            (int)Math.Ceiling((double)totalCount / query.ResultsPerPage), totalCount);
    }

    public async Task<Subject> GetAsync(SubjectId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Subjects
            .Where(s => s.Id == id)
            .SingleAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(SubjectId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Subjects
            .AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task AddAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        await _dbContext.Subjects.AddAsync(subject, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        _dbContext.Subjects.Update(subject);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(SubjectId id, CancellationToken cancellationToken = default)
    {
        var subject = await _dbContext.Subjects
            .SingleAsync(s => s.Id.Value == id.Value, cancellationToken);
        _dbContext.Subjects.Remove(subject);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Extended Subject queries
    public async Task<Subject> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Subjects
            .Where(s => s.Email.Value == email)
            .SingleAsync(cancellationToken);
    }

    public async Task<Subject> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Subjects
            .Where(s => s.Email.Value == email.Value)
            .SingleAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Subject>> GetByStatusAsync(SubjectStatus status, CancellationToken cancellationToken = default)
    {
        var subjects = await _dbContext.Subjects
            .Where(s => s.Status == status)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(subjects);
    }

    public async Task<IReadOnlyList<Subject>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        // Query subjects where the JSONB roles array contains the role ID
        var subjects = await _dbContext.Subjects
            .Where(s => s.Roles.Any(r => r.Value == roleId.Value))
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(subjects);
    }

    public async Task<IReadOnlyList<Subject>> GetByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        var subjects = await _dbContext.Subjects
            .Where(s => s.Tags.Contains(tag))
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(subjects);
    }

    public async Task<IReadOnlyList<Subject>> GetActiveSubjectsAsync(CancellationToken cancellationToken = default)
    {
        var subjects = await _dbContext.Subjects
            .Where(s => s.Status == SubjectStatus.Active)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(subjects);
    }

    public async Task<IReadOnlyList<Subject>> GetRecentlyAuthenticatedAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        var subjects = await _dbContext.Subjects
            .Where(s => s.LastAuthenticatedAt.HasValue && s.LastAuthenticatedAt.Value >= since)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(subjects);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Subjects
            .AnyAsync(s => s.Email.Value == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, SubjectId excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Subjects
            .AnyAsync(s => s.Email.Value == email && s.Id.Value != excludeId.Value, cancellationToken);
    }

    public async Task<bool> HasRoleAsync(SubjectId subjectId, RoleId roleId, CancellationToken cancellationToken = default)
    {
        var subject = await _dbContext.Subjects
            .Where(s => s.Id.Value == subjectId.Value)
            .SingleAsync(cancellationToken);
        return subject.Roles.Any(r => r.Value == roleId.Value);
    }

    public async Task<bool> HasTagAsync(SubjectId subjectId, string tag, CancellationToken cancellationToken = default)
    {
        var subject = await _dbContext.Subjects
            .Where(s => s.Id.Value == subjectId.Value)
            .SingleAsync(cancellationToken);
        return subject.Tags.Contains(tag);
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}