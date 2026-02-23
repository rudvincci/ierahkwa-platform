namespace Mamey.Portal.Citizenship.Application.Models;

public sealed record StatusProgressionTimelineDto(
    IReadOnlyList<StatusProgressionTimelineEntryDto> Entries);

public sealed record StatusProgressionTimelineEntryDto(
    DateTimeOffset Timestamp,
    string Event,
    string Description,
    string Status,
    string? Details = null);

