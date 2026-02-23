using Mamey.Postgres;

namespace Mamey.Government.Modules.Documents.Core.EF;

internal class DocumentsUnitOfWork : PostgresUnitOfWork<DocumentsDbContext>, IDocumentsUnitOfWork
{
    private readonly DocumentsDbContext _dbContext;

    public DocumentsUnitOfWork(DocumentsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
