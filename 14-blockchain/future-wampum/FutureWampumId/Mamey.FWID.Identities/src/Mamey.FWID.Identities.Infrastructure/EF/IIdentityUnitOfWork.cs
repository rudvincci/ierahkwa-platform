using System.Threading;
using System.Threading.Tasks;
using Mamey.Persistence.SQL;

namespace Mamey.FWID.Identities.Infrastructure.EF
{
    internal interface IIdentityUnitOfWork : IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}