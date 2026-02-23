using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Certificates.Core.DTO;
using Mamey.Government.Modules.Certificates.Core.Mappings;
using Mamey.MicroMonolith.Abstractions;
using Mamey.Types;

namespace Mamey.Government.Modules.Certificates.Core.Queries.Handlers;

internal sealed class BrowseCertificatesHandler : IQueryHandler<BrowseCertificates, PagedResult<CertificateSummaryDto>>
{
    private readonly ICertificateRepository _repository;

    public BrowseCertificatesHandler(ICertificateRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CertificateSummaryDto>> HandleAsync(BrowseCertificates query, CancellationToken cancellationToken = default)
    {
        var certificates = await _repository.GetByTenantAsync(new TenantId(query.TenantId), cancellationToken);
        
        var filtered = certificates.AsEnumerable();

        // Filter by certificate type
        if (!string.IsNullOrWhiteSpace(query.CertificateType) && 
            Enum.TryParse<CertificateType>(query.CertificateType, true, out var type))
        {
            filtered = filtered.Where(c => c.CertificateType == type);
        }

        // Filter by status
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            filtered = query.Status.ToLower() switch
            {
                "active" => filtered.Where(c => c.IsActive),
                "revoked" => filtered.Where(c => !c.IsActive),
                "archived" => filtered.Where(c => !c.IsActive && c.RevocationReason?.Contains("Archived") == true),
                _ => filtered
            };
        }

        // Search by certificate number
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            filtered = filtered.Where(c => 
                c.CertificateNumber.ToLower().Contains(term));
        }

        var list = filtered.ToList();
        var totalCount = list.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        var items = list
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => c.AsSummaryDto())
            .ToList();

        return PagedResult<CertificateSummaryDto>.Create(items, query.Page, query.PageSize, totalPages, totalCount);
    }
}
