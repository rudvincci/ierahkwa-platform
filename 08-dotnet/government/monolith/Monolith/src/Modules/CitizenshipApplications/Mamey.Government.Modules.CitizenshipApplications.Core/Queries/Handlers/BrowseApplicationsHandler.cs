using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Mamey.Types;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Queries.Handlers;

internal sealed class BrowseApplicationsHandler : IQueryHandler<BrowseApplications, PagedResult<ApplicationSummaryDto>>
{
    private readonly IApplicationRepository _repository;
    private readonly IContext _context;

    public BrowseApplicationsHandler(IApplicationRepository repository, IContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<PagedResult<ApplicationSummaryDto>> HandleAsync(BrowseApplications query, CancellationToken cancellationToken = default)
    {
        // Get all applications for the tenant
        var applications = await _repository.GetByTenantAsync(_context.TenantId, cancellationToken);

        // Apply filters in memory
        IEnumerable<CitizenshipApplication> filtered = applications;

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (Enum.TryParse<ApplicationStatus>(query.Status, out var status))
            {
                filtered = filtered.Where(a => a.Status == status);
            }
        }

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchLower = query.SearchTerm.ToLower();
            filtered = filtered.Where(a =>
                a.FirstName.ToLower().Contains(searchLower) ||
                a.LastName.ToLower().Contains(searchLower) ||
                a.ApplicationNumber.Value.ToLower().Contains(searchLower));
        }

        if (query.FromDate.HasValue)
        {
            filtered = filtered.Where(a => a.CreatedAt >= query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            filtered = filtered.Where(a => a.CreatedAt <= query.ToDate.Value);
        }

        // Calculate pagination
        var totalCount = filtered.Count();
        var page = query.Page > 0 ? query.Page : 1;
        var pageSize = query.PageSize > 0 ? query.PageSize : 10;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var pagedItems = filtered
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var items = pagedItems.Select(a => new ApplicationSummaryDto(
            a.Id.Value,
            a.ApplicationNumber.Value,
            $"{a.FirstName} {a.LastName}",
            a.Status.ToString(),
            a.Step.ToString(),
            a.CreatedAt,
            a.SubmittedAt,
            a.UpdatedAt,
            a.Email?.Value)).ToList();

        return PagedResult<ApplicationSummaryDto>.Create(
            items,
            page,
            pageSize,
            totalPages,
            totalCount);
    }
}
