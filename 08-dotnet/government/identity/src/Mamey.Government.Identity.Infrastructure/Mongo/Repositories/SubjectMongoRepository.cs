using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Repositories;

internal class SubjectMongoRepository : ISubjectRepository
{
    private readonly IMongoRepository<SubjectDocument, Guid> _repository;

    public SubjectMongoRepository(IMongoRepository<SubjectDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Subject subject, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new SubjectDocument(subject));

    public async Task UpdateAsync(Subject subject, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new SubjectDocument(subject));
    public async Task DeleteAsync(SubjectId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Subject>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();

    public Task<PagedResult<Subject>> BrowseAsync(Func<Subject, bool> predicate, PagedQueryBase query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Subject> GetAsync(SubjectId id, CancellationToken cancellationToken = default)
    {
        var subject = await _repository.GetAsync(id.Value);
        return subject?.AsEntity();
    }
    public async Task<bool> ExistsAsync(SubjectId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);

    public Task<Subject> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Subject>> GetByStatusAsync(SubjectStatus status, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Subject>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Subject>> GetByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Subject>> GetActiveSubjectsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Subject>> GetRecentlyAuthenticatedAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EmailExistsAsync(string email, SubjectId excludeId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasRoleAsync(SubjectId subjectId, RoleId roleId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasTagAsync(SubjectId subjectId, string tag, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Subject> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}



