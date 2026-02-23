using Mamey.Persistence.SQL;

namespace Mamey.Government.Modules.Documents.Core.EF;

internal interface IDocumentsUnitOfWork : IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
