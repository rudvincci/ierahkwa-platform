using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;
using Mamey.Government.Modules.CitizenshipApplications.Core.Exceptions;
using ApplicationId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Queries.Handlers;

internal sealed class GetApplicationTimelineHandler : IQueryHandler<GetApplicationTimeline, IReadOnlyList<ApplicationTimelineEntryDto>>
{
    private readonly IApplicationRepository _repository;

    public GetApplicationTimelineHandler(IApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ApplicationTimelineEntryDto>> HandleAsync(GetApplicationTimeline query, CancellationToken cancellationToken = default)
    {
        var application = await _repository.GetAsync(new ApplicationId(query.ApplicationId), cancellationToken);
        if (application is null)
        {
            throw new ApplicationNotFoundException(query.ApplicationId);
        }

        var timeline = new List<ApplicationTimelineEntryDto>();

        // Add creation event
        timeline.Add(new ApplicationTimelineEntryDto
        {
            Event = "Application Created",
            Description = $"Application {application.ApplicationNumber} was created",
            Status = ApplicationStatus.Draft.ToString(),
            Timestamp = application.CreatedAt,
            Actor = "System"
        });

        // Add status progression events based on current state
        if (application.Status >= ApplicationStatus.Submitted)
        {
            timeline.Add(new ApplicationTimelineEntryDto
            {
                Event = "Application Submitted",
                Description = "Application submitted for processing",
                Status = ApplicationStatus.Submitted.ToString(),
                Timestamp = application.UpdatedAt // Would need status history for exact time
            });
        }

        // If approved, add approval event
        if (application.Status == ApplicationStatus.Approved && application.ApprovedAt.HasValue)
        {
            timeline.Add(new ApplicationTimelineEntryDto
            {
                Event = "Application Approved",
                Description = "Application was approved",
                Status = ApplicationStatus.Approved.ToString(),
                Timestamp = application.ApprovedAt.Value,
                Actor = application.ApprovedBy
            });
        }

        // If rejected, add rejection event
        if (application.Status == ApplicationStatus.Rejected && application.RejectedAt.HasValue)
        {
            timeline.Add(new ApplicationTimelineEntryDto
            {
                Event = "Application Rejected",
                Description = application.RejectionReason ?? "Application was rejected",
                Status = ApplicationStatus.Rejected.ToString(),
                Timestamp = application.RejectedAt.Value,
                Actor = application.RejectedBy
            });
        }

        // Add document upload events
        foreach (var upload in application.Uploads.OrderBy(u => u.UploadedAt))
        {
            timeline.Add(new ApplicationTimelineEntryDto
            {
                Event = "Document Uploaded",
                Description = $"Uploaded {upload.DocumentType}: {upload.FileName}",
                Status = application.Status.ToString(),
                Timestamp = upload.UploadedAt
            });
        }

        return timeline.OrderBy(t => t.Timestamp).ToList();
    }
}
